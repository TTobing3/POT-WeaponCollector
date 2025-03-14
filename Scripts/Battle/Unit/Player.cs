using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] WeaponSprite weaponSprite;

    UnitEffect unitEffect;

    public int heart, maxHeart, armor;
    public Enemy enemy;
    public bool onDie = false;

    public int health = 0, magic = 0, gold = 0;

    public int[] trains = new int[16], level = new int[16];

    public List<string> buffList;

    private void Awake()
    {
        unitEffect = GetComponent<UnitEffect>();
    }
    void Start()
    {
        UIManager.instance.UpdatePlayerUI(heart, armor);
        UIManager.instance.UpdateCostUI();

        StartSet();
    }

    void StartSet()
    {
        print("----- Player ���� ���� ���� -----");

        //�ʱ� �ڵ�/�� : 1/1 (�߰����� ���� ���� ����)
        print("�ʱ� �ڵ�/�� : 2/4");
        StageManager.instance.hand.AddHandSlot();
        StageManager.instance.hand.AddHandSlot();

        StageManager.instance.deck.AddDeckSlot();
        StageManager.instance.deck.AddDeckSlot();
        StageManager.instance.deck.AddDeckSlot();
        StageManager.instance.deck.AddDeckSlot();

        //�ʱ� ��Ʈ/���� : 1/0 (�߰����� ���� ���� ����)
        print("��Ʈ/���� : 3/0");
        AddMaxHeart();
        AddMaxHeart();
        AddMaxHeart();
        AddHeart();
        AddHeart();
        AddHeart();

        //�ʱ� ü��/���� : 1/1 (�߰����� ���� ���� ����)
        print("ü��/���� : 2/2");
        AddHealth(2);
        AddMagic(2);

        //��� �迭 ���õ� : 0 (�߰����� ���� ���� ����)
        print("�� ���õ� : 1");
        ChangeTrain(0, 1);

        print("----- Player ���� ���� �� -----");
    }

    public void ResetState()
    {
        armor = 0;
        heart = maxHeart;
        UIManager.instance.UpdatePlayerUI(heart, armor);

        buffList = new List<string>();
        UIManager.instance.UpdatePlayerBuff(buffList);
    }

    #region Act
    public void ActSkill(Enemy enemy)
    {
        this.enemy = enemy;

        UseWeapon(0);
    }
    void UseWeapon(int number)
    {
        // ��� ���� ��� �� �� ����
        if (number == Hand.instance.cards.Count + Hand.instance.tempHandSlots.Count)
        {
            TurnManager.instance.FinishPlayerSkill();
            return;
        }

        DeckSlot deckSlot;

        //����
        if (number < Hand.instance.cards.Count)
        {
            if (Hand.instance.cards[number].deckSlot == null) // �� ĭ
            {
                UseWeapon(number + 1);
                return;
            }

            deckSlot = Hand.instance.cards[number].deckSlot;

        }
        else
        {
            deckSlot = Hand.instance.tempHandSlots[number - Hand.instance.cards.Count].deckSlot;
        }

        EventManager.instance.UseWeapon(deckSlot, () => { ActWeapon(number, deckSlot); });
    }

    void ActWeapon(int number, DeckSlot deckSlot)
    {
        weaponSprite.Attack(deckSlot);

        var playerAnimator = transform.GetChild(0).GetComponent<Animator>();

        playerAnimator.SetInteger("state", 2);
        playerAnimator.SetTrigger("act");

        var pos = -5;

        transform.DOMoveX(pos + 1, 0.2f).SetDelay(0.3f).SetEase(Ease.InQuad).OnComplete(() => {
            transform.DOMoveX(pos, 0.8f);
            StartCoroutine(ActEffect(deckSlot, () => { UseWeapon(number + 1); }));
        });
    }

    IEnumerator ActEffect(DeckSlot deckSlot, System.Action action) // deckSlot - ��ȭ �� �ΰ�ȿ�� ������ ���
    {
        foreach(string i in deckSlot.GetWeaponData.skills)
        {

            var skill = DataManager.instance.AllSkillDatas[i];

            // ����
            if(!SkillCaculator.Chance(skill.chance + deckSlot.upgrade.luck + deckSlot.turnUpgrade.luck + deckSlot.roundUpgrade.luck))
            {
                EventManager.instance.FailedActEffect(deckSlot, () =>
                {
                    FloatText($"{skill.name} ����!");
                });
            }
            // ����
            else 
            {
                EventManager.instance.SuccessActEffect(deckSlot, () =>
                {
                    switch (skill.effectType)
                    {
                        case "����":
                            AttackEnemy(int.Parse(SkillCaculator.Caculate(skill.effectType, deckSlot.GetWeaponPower, skill.effectPower)));
                            enemy.GetComponent<UnitEffect>().HitEffect(skill.effectSprite);
                            break;
                        case "����":
                            AddArmor(int.Parse(SkillCaculator.Caculate(skill.effectType, deckSlot.GetWeaponPower, skill.effectPower)));
                            unitEffect.HitEffect(skill.effectSprite);
                            break;
                        case "ȸ��":
                            AddHeart(int.Parse(SkillCaculator.Caculate(skill.effectType, deckSlot.GetWeaponPower, skill.effectPower)));
                            unitEffect.HitEffect(skill.effectSprite);
                            break;
                        case "��":
                            print($"����(��) ���!");
                            enemy.GetComponent<UnitEffect>().HitEffect(skill.effectSprite);
                            break;
                        case "����":
                            GiveBuff(skill.effectType, int.Parse(SkillCaculator.Caculate(skill.effectType, deckSlot.GetWeaponPower, skill.effectPower)));
                            enemy.GetComponent<UnitEffect>().HitEffect(skill.effectSprite);
                            break;
                        case "����":
                            for(int i = 0; i< (int)skill.effectPower; i++)
                               Deck.instance.AddDeckSlot();
                            break;
                        case "����":
                            AddMaxHeart((int)skill.effectPower);
                            break;
                        case "ü��":
                            AddHealth((int)skill.effectPower);
                            break;
                        case "����":
                            AddMagic((int)skill.effectPower);
                            break;

                        case "�Ҹ�":
                            print($"{deckSlot.name}�� �Һ��Ͽ����ϴ�!");
                            deckSlot.Erase();
                            break;
                    }
                });
            }

            yield return new WaitForSeconds(0.5f);
        }

        action();
    }

    //����
    #endregion

    #region Skill
    void AttackEnemy(int damage)
    {
        EventManager.instance.AttackEnemy(damage, ()=> { enemy.TakeDamage(damage); });
    }
    public void TakeDamage(int damage)
    {
        HitAnimation();
        HitWhiteFlash();
        FloatDamage(damage);
        UIManager.instance.ShakeCameraByDamage(damage);

        if (armor > 0)
        {
            int remainingDamage = damage - armor;
            armor -= damage;

            if (armor < 0)
            {
                armor = 0;
            }

            if (remainingDamage > 0)
            {
                DecreaseHeart();
            }
        }
        else
        {
            DecreaseHeart();
        }

        if (heart <= 0)
        {
            heart = 0;
            Die();
        }

        UIManager.instance.UpdatePlayerUI(heart, armor);
    }

    public void FloatText(string text)
    {
        UIManager.instance
            .FloatText(new FloatData(text, new Vector3(-550, -30)) // 10 ������ 1.1��
            .SetFontsize(64 * (1 + 1 / 10 * 0.1f))
            .SetDirection(new float[] { -50, 150 }));
    }

    public void FloatDamage(int damage)
    {
        UIManager.instance
            .FloatDamage(new FloatData($"-{damage}", new Vector3(-550, -30)) // 10 ������ 1.1��
            .SetFontsize(64 * (1 + damage / 10 * 0.1f))
            .SetDirection(new float[] { -50, 150 }));
    }


    public void HitAnimation()
    {
        transform.DORotate(new Vector3(0, 0, 10), 0.1f)
            .OnComplete(() => transform.DORotate(new Vector3(0, 0, 0), 0.1f));
        transform.DOMoveX(transform.position.x - 0.3f, 0.1f).SetDelay(0.05f)
            .OnComplete(() => transform.DOMoveX(transform.position.x + 0.3f, 0.45f));
    }

    public void HitWhiteFlash()
    {
        unitEffect.HitWhiteFlash();
    }

    void GiveBuff(string buff, int power)
    {
        if(Random.Range(0,100) < power)
        {
            enemy.GainBuff(buff);
        }
        else
        {
            print($"{buff} �ο� ����");
        }
    }

    void ChangeArmor(int armor)
    {
        this.armor = armor;
        UIManager.instance.UpdatePlayerUI(heart, this.armor);
    }

    void AddArmor(int armor = 1)
    {
        this.armor += armor;
        UIManager.instance.UpdatePlayerUI(heart, this.armor);
    }
    #endregion

    #region Status & Gold

    // ��ȭ

    public void ChangeGold(int gold)
    {
        this.gold += gold;
        UIManager.instance.UpdateGold(this.gold);
    }

    // �ڽ�Ʈ

    public void AddHealth(int health)
    {
        this.health += health;
        UIManager.instance.UpdateCostUI();
    }

    public void AddMagic(int magic)
    {
        this.magic += magic;
        UIManager.instance.UpdateCostUI();
    }

    // ���õ�
    public void ChangeTrain(int type, int train)
    {
        trains[type] += train;
    }
    public void AddTrain(int type)
    {
        trains[type]++;
    }

    public TrainLevelData GetTrainLevel(string type)
    {

        var weaponType = (int)DataManager.instance.GetWeaponType(type);
        TrainLevelData playerTrainLevel = null;

        foreach (TrainLevelData j in DataManager.instance.TrainLevelTable)
        {
            if (trains[weaponType] < j.requireTrain) break;
            playerTrainLevel = j;
        }

        return playerTrainLevel;
    }

    // ����
    public void ChangeHeart(int heart)
    {
        this.heart = maxHeart < heart ? maxHeart : heart;
        UIManager.instance.UpdatePlayerUI(this.heart, armor);
    }

    public void AddHeart(int heart = 1)
    {
        this.heart = maxHeart < (this.heart + heart) ? maxHeart : (this.heart + heart);
        UIManager.instance.UpdatePlayerUI(this.heart, armor);
    }
    public void DecreaseHeart()
    {
        heart--;
        UIManager.instance.UpdatePlayerUI(heart, armor);
    }

    public void ChangeMaxHeart(int heart)
    {
        maxHeart += heart;
        UIManager.instance.UpdatePlayerUI(heart, armor);
    }

    public void AddMaxHeart(int heart = 1)
    {
        maxHeart += heart;
        UIManager.instance.UpdatePlayerUI(this.heart, armor);
    }

    // ����
    public void AddArmor()
    {

    }

    #endregion

    #region Buff

    public void GainBuff(string buff, System.Action gainEffect = null)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].Contains(buff))
            {
                buffList[i] = $"{buff}:{int.Parse(buffList[i].Split(':')[1]) + 1} ";
                UIManager.instance.UpdatePlayerBuff(buffList);
                return;
            }
        }
        buffList.Add($"{buff}:1");
        UIManager.instance.UpdatePlayerBuff(buffList);

        if(gainEffect != null)
            gainEffect();
    }
    public bool CheckBuff(string buff)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].Contains(buff) && int.Parse(buffList[i].Split(':')[1]) != 0)
            {
                print($"{buff} : {buffList[i].Split(':')[1]}");
                return true;
            }
        }
        return false;
    }

    public void DecreaseBuff(string buff)
    {
        //buff : ������
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].Contains(buff) && int.Parse(buffList[i].Split(':')[1]) != 0)
            {
                buffList[i] = $"{buff}:{int.Parse(buffList[i].Split(':')[1]) - 1} ";
            }
        }
        UIManager.instance.UpdatePlayerBuff(buffList);
    }

    public void EraseEmptyBuff()
    {
        buffList.RemoveAll(x => x.Split(":")[1].Trim() == "0");
        UIManager.instance.UpdatePlayerBuff(buffList);
    }

    #endregion

    void Die()
    {
        onDie = true;

        OutPlayer();

        StageManager.instance.GameOver();
    }

    void OutPlayer()
    {
        GetComponent<SpriteRenderer>().DOFade(0, 0.5f);
    }
}
