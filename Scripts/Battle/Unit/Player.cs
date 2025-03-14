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
        print("----- Player 시작 세팅 시작 -----");

        //초기 핸드/덱 : 1/1 (추가하지 않은 최초 상태)
        print("초기 핸드/덱 : 2/4");
        StageManager.instance.hand.AddHandSlot();
        StageManager.instance.hand.AddHandSlot();

        StageManager.instance.deck.AddDeckSlot();
        StageManager.instance.deck.AddDeckSlot();
        StageManager.instance.deck.AddDeckSlot();
        StageManager.instance.deck.AddDeckSlot();

        //초기 하트/갑옷 : 1/0 (추가하지 않은 최초 상태)
        print("하트/갑옷 : 3/0");
        AddMaxHeart();
        AddMaxHeart();
        AddMaxHeart();
        AddHeart();
        AddHeart();
        AddHeart();

        //초기 체력/마력 : 1/1 (추가하지 않은 최초 상태)
        print("체력/마력 : 2/2");
        AddHealth(2);
        AddMagic(2);

        //모든 계열 숙련도 : 0 (추가하지 않은 최초 상태)
        print("검 숙련도 : 1");
        ChangeTrain(0, 1);

        print("----- Player 시작 세팅 끝 -----");
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
        // 모든 무기 사용 시 턴 종료
        if (number == Hand.instance.cards.Count + Hand.instance.tempHandSlots.Count)
        {
            TurnManager.instance.FinishPlayerSkill();
            return;
        }

        DeckSlot deckSlot;

        //세팅
        if (number < Hand.instance.cards.Count)
        {
            if (Hand.instance.cards[number].deckSlot == null) // 빈 칸
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

    IEnumerator ActEffect(DeckSlot deckSlot, System.Action action) // deckSlot - 강화 등 부가효과 때문에 사용
    {
        foreach(string i in deckSlot.GetWeaponData.skills)
        {

            var skill = DataManager.instance.AllSkillDatas[i];

            // 실패
            if(!SkillCaculator.Chance(skill.chance + deckSlot.upgrade.luck + deckSlot.turnUpgrade.luck + deckSlot.roundUpgrade.luck))
            {
                EventManager.instance.FailedActEffect(deckSlot, () =>
                {
                    FloatText($"{skill.name} 실패!");
                });
            }
            // 성공
            else 
            {
                EventManager.instance.SuccessActEffect(deckSlot, () =>
                {
                    switch (skill.effectType)
                    {
                        case "피해":
                            AttackEnemy(int.Parse(SkillCaculator.Caculate(skill.effectType, deckSlot.GetWeaponPower, skill.effectPower)));
                            enemy.GetComponent<UnitEffect>().HitEffect(skill.effectSprite);
                            break;
                        case "갑옷":
                            AddArmor(int.Parse(SkillCaculator.Caculate(skill.effectType, deckSlot.GetWeaponPower, skill.effectPower)));
                            unitEffect.HitEffect(skill.effectSprite);
                            break;
                        case "회복":
                            AddHeart(int.Parse(SkillCaculator.Caculate(skill.effectType, deckSlot.GetWeaponPower, skill.effectPower)));
                            unitEffect.HitEffect(skill.effectSprite);
                            break;
                        case "독":
                            print($"독을(를) 사용!");
                            enemy.GetComponent<UnitEffect>().HitEffect(skill.effectSprite);
                            break;
                        case "공포":
                            GiveBuff(skill.effectType, int.Parse(SkillCaculator.Caculate(skill.effectType, deckSlot.GetWeaponPower, skill.effectPower)));
                            enemy.GetComponent<UnitEffect>().HitEffect(skill.effectSprite);
                            break;
                        case "가방":
                            for(int i = 0; i< (int)skill.effectPower; i++)
                               Deck.instance.AddDeckSlot();
                            break;
                        case "생명":
                            AddMaxHeart((int)skill.effectPower);
                            break;
                        case "체력":
                            AddHealth((int)skill.effectPower);
                            break;
                        case "마력":
                            AddMagic((int)skill.effectPower);
                            break;

                        case "소모":
                            print($"{deckSlot.name}를 소비하였습니다!");
                            deckSlot.Erase();
                            break;
                    }
                });
            }

            yield return new WaitForSeconds(0.5f);
        }

        action();
    }

    //피해
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
            .FloatText(new FloatData(text, new Vector3(-550, -30)) // 10 단위로 1.1배
            .SetFontsize(64 * (1 + 1 / 10 * 0.1f))
            .SetDirection(new float[] { -50, 150 }));
    }

    public void FloatDamage(int damage)
    {
        UIManager.instance
            .FloatDamage(new FloatData($"-{damage}", new Vector3(-550, -30)) // 10 단위로 1.1배
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
            print($"{buff} 부여 실패");
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

    // 금화

    public void ChangeGold(int gold)
    {
        this.gold += gold;
        UIManager.instance.UpdateGold(this.gold);
    }

    // 코스트

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

    // 숙련도
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

    // 생명
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

    // 갑옷
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
        //buff : 버프명
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
