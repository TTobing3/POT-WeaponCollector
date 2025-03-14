using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;

    public EnemyInterect enemyInterect;

    public Tooltipable tooltipable;
    public int maxHealth, health, armor;
    public bool onDie = false;

    Player player;
    SpriteResolver spriteResolver;
    UnitEffect unitEffect;

    public int patternIndex = 0;

    public List<string> buffList;

    void Awake()
    {
        spriteResolver = GetComponent<SpriteResolver>();
        unitEffect = GetComponent<UnitEffect>();
    }


    public void Set(EnemyData enemyData)
    {
        this.enemyData = enemyData;

        tooltipable.data = "Character:" + enemyData.name;

        patternIndex = 0;

        onDie = false;

        maxHealth = enemyData.health;
        health = enemyData.health;

        armor = enemyData.armor;

        buffList = new List<string>();

        spriteResolver.SetCategoryAndLabel("Character", enemyData.name);

        UIManager.instance.UpdateEnemyUI(health, armor, maxHealth);
        UIManager.instance.UpdateEnemyBuff(buffList);

    }

    #region Act
    public void ActSkill(Player player)
    {
        this.player = player;

        UseSkill();

        patternIndex = patternIndex == enemyData.pattern.Count - 1 ? 0 : patternIndex + 1;
    }

    void UseSkill()
    {
        var pos = 5;

        transform.DOMoveX(pos - 1, 0.2f).SetEase(Ease.InQuad).OnComplete(() => {
            ActEffect(enemyData.pattern[patternIndex]);
            player.GetComponent<UnitEffect>().HitEffect(DataManager.instance.AllEnemySkillDatas[enemyData.pattern[patternIndex]].effect);
            transform.DOMoveX(pos, 0.8f).OnComplete(() => {
                TurnManager.instance.FinishEnemySkill();
                enemyInterect.KeepFocus();
            });
        });
    }



    void ActEffect(string _skill)
    {
        var skill = DataManager.instance.AllEnemySkillDatas[_skill];

        var e = skill.effects[0].Split(":");
        switch (e[0])
        {
            case "피해":
                AttackPlayer(int.Parse(e[1]));
                break;
            case "방어":
                GetArmor(int.Parse(e[1]));
                break;
            case "공포":
                GiveBuff(e[0], int.Parse(e[1]));
                break;
        }
    }
    #endregion

    #region Skill
    void AttackPlayer(int damage)
    {
        player.TakeDamage(damage);
    }
    void GiveBuff(string buff, int power)
    {
        if (Random.Range(0, 100) < power)
        {
            player.GainBuff(buff);
        }
        else
        {
            print($"{buff} 부여 실패");
        }
    }

    void GetArmor(int armor)
    {
        this.armor += armor;
    }
    #endregion

    #region Buff
    public void GainBuff(string buff)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].Contains(buff))
            {
                buffList[i] = $"{buff}:{int.Parse(buffList[i].Split(':')[1]) + 1} ";
                UIManager.instance.UpdateEnemyBuff(buffList);
                return;
            }
        }
        buffList.Add($"{buff}:1");
        UIManager.instance.UpdateEnemyBuff(buffList);
    }
    public bool CheckBuff(string buff)
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].Contains(buff) && buffList[i].Split(':')[1] != "0")
            {
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
        UIManager.instance.UpdateEnemyBuff(buffList);
    }

    public void EraseEmptyBuff()
    {
        buffList.RemoveAll(x => x.Split(":")[1].Trim() == "0");
        UIManager.instance.UpdateEnemyBuff(buffList);
    }

    #endregion

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
                health -= remainingDamage;
            }
        }
        else
        {
            health -= damage;
        }

        if (health <= 0)
        {
            health = 0;
            Die();
        }

        UIManager.instance.UpdateEnemyUI(health, armor, maxHealth);
    }

    public void FloatText(string text)
    {
        UIManager.instance
            .FloatText(new FloatData(text, new Vector3(600, -30)) // 10 단위로 1.1배
            .SetFontsize(64 * (1 + 1 / 10 * 0.1f))
            .SetDirection(new float[] { 50, 150 }));
    }

    public void FloatDamage(int damage)
    {
        UIManager.instance
            .FloatDamage(new FloatData($"-{damage}", new Vector3(600, -30)) // 10 단위로 1.1배
            .SetFontsize(64 * (1 + damage / 10 * 0.1f))
            .SetDirection(new float[] { 50, 150 }));
    }

    void HitAnimation()
    {
        transform.DORotate(new Vector3(0, 0, -10), 0.1f)
            .OnComplete(() => transform.DORotate(new Vector3(0, 0, 0), 0.1f));
        transform.DOMoveX(transform.position.x + 0.3f, 0.1f).SetDelay(0.05f)
            .OnComplete(() => transform.DOMoveX(transform.position.x - 0.3f, 0.45f));
    }

    public void HitWhiteFlash()
    {
        unitEffect.HitWhiteFlash();
    }


    void Die()
    {
        onDie = true;
        // 적이 죽었을 때의 처리 로직 (예: 사망 애니메이션, 오브젝트 비활성화 등)
        // 추가적으로 보상 로직이나 게임 상태 업데이트 등을 넣을 수 있음
    }


}
