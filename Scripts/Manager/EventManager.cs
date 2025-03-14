using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    #region Event

    #region Deck

    public void EquipWeapon(System.Action next)
    {
        if (next != null)
            next();
    }

    #endregion

    #region Player

    public void StartPlayerAct(System.Action next)
    {
        // 턴 스킵 시, 작동 할 효과는 공포 이전에 정의

        #region 버프

        EffectBuff(true, "공포", () =>
        {
            TurnManager.instance.FinishPlayerSkill();
            next = null;

            return;
        });
        
        #endregion

        // 턴 스킵 시, 작동 안 할 효과는 공포 이전에 정의

        #region 소속
        //자연
        ActNatureSynergy();

        //모험
        ActAdventurerSynergy();

        // 용병
        ActMercenarySynergy();

        #endregion

        if (next != null)
            next();
    }

    // 턴 종료 시
    public void FinishPlayerTurn(System.Action next)
    {
        EffectBuff(true, "적응"     , () => {});
        EffectBuff(true, "완전 적응", () => {});
        EffectBuff(true, "만족", () => { });


        if (next != null)
            next();
    }

    // 무기 기술 위력 설정 시
    public float SetEffectRange(int[] range, System.Func<float> next)
    {
        float power = 0;

        if (next != null) power = next();

        EffectBuff(true, "정자세", () =>
        {
            power = range[1];
        });

        return power;
    }

    // 무기 사용 시 ( 성공 실패 이전 )
    public void UseWeapon(DeckSlot deckSlot, System.Action next) // 플레이어 무기 하나 사용
    {
        #region 소속

        // 지옥
        ActHellSynergy(deckSlot);

        #endregion

        #region 버프

        EffectBuff(true, "파멸", () =>
        {
            if(Deck.instance.deckWeaponList.Count > 1)
            {
                var exceptIndex = Deck.instance.deckWeaponList.IndexOf(deckSlot);

                int targetIndex = 0;

                do
                {
                    targetIndex = Random.Range(0, Deck.instance.deckWeaponList.Count);
                }
                while (exceptIndex == targetIndex);

                Deck.instance.deckWeaponList[targetIndex].Erase();
            }
        });


        EffectBuff(true, "훈련", () => 
        {
            var type = (int)DataManager.instance.GetWeaponType(deckSlot.GetWeaponData.type);
            TurnManager.instance.player.AddTrain(type);
        });

        EffectBuff(true, "갈무리", () =>
        {
            print("갈무리 작동");

            var chance = Random.Range(0, 100);

            print($"확률 { chance }");

            if ( chance < 5)
            {
                GainMercenaryWeapon("귀중한 전리품");
            }
            else if (chance < 20)
            {
                GainMercenaryWeapon("가치있는 전리품");
            }
            else if (chance < 50)
            {
                GainMercenaryWeapon("흔한 전리품");
            }
            else
            {
                GainMercenaryWeapon("쓸데없는 전리품");
            }

            void GainMercenaryWeapon(string type)
            {
                var spoils = (from weapon in DataManager.instance.AllWeaponDataList
                                        where weapon.rank == type
                                        select weapon).ToList();

                var spoilsWeapon = spoils[Random.Range(0, spoils.Count)];

                Deck.instance.GainWeapon(spoilsWeapon.name);
            }

        });

        #endregion

        if (next != null)
            next();
    }


    // 공격 실패 시
    public void FailedActEffect(DeckSlot deckSlot, System.Action next)
    {
        #region 소속
        #endregion

        #region 버프
        #endregion

        if (next != null)
            next();
    }

    // 공격 성공 시
    public void SuccessActEffect(DeckSlot deckSlot, System.Action next)
    {
        #region 소속

        //제국
        ActEmpireSynergy(deckSlot);

        #endregion

        #region 버프

        #endregion

        if (next != null)
            next();
    }

    public void AttackEnemy(int damage, System.Action next)
    {
        EffectBuff(true, "긍지", () =>
        {
            TurnManager.instance.enemy.TakeDamage((int)(damage*1.5f));
            next = null;
        });

        EffectBuff(true ,"압도", () =>
        {
            if (Random.Range(0, 100) < 50)
            {
                TurnManager.instance.enemy.GainBuff("공포");
            }
            else
            {
                print($"압도 실패");
            }
        });

        if (next != null)
            next();
    }

    #endregion

    #region Enemy

    public void StartEnemyAct(System.Action next)
    {
        EffectBuff(false, "공포", () =>
        {
            TurnManager.instance.FinishEnemySkill();
            next = null;
        });

        if(next != null)
            next();
    }

    public void FinishEnemyTurn(System.Action next)
    {

        if (next != null)
            next();
    }

    #endregion

    #endregion

    #region GroupSynergyEffect

    // 자연
    void ActNatureSynergy()
    {
        GroupSynergy groupSynergy = Hand.instance.GetSynergy("자연");

        if (groupSynergy == null) return;


        //효과
        if (groupSynergy.count >= 5)
        {
            AddNatureWeapon("강력한 자연");
            AddNatureWeapon("강력한 자연");
        }
        else if (groupSynergy.count >= 4)
        {
            AddNatureWeapon("강력한 자연");
        }
        else if (groupSynergy.count >= 3)
        {
            AddNatureWeapon("자연");
            AddNatureWeapon("자연");
        }
        else if (groupSynergy.count >= 2)
        {
            AddNatureWeapon("자연");
        }

        void AddNatureWeapon(string type)
        {
            var natureWeaponList = (from weapon in DataManager.instance.AllWeaponDataList
                                    where weapon.rank == type
                                    select weapon).ToList();

            var natureWeapon = natureWeaponList[Random.Range(0, natureWeaponList.Count)];
            Hand.instance.AddTempWeapon(natureWeapon.name);
        }

    }

    // 제국
    void ActEmpireSynergy(DeckSlot deckSlot)
    {
        GroupSynergy groupSynergy = Hand.instance.GetSynergy("제국");

        if (groupSynergy == null) return;


        //효과
        if (groupSynergy.count >= 4)
        {
            TurnManager.instance.player.GainBuff("정자세");
        }
        else if (groupSynergy.count >= 3)
        {
            // 훈련
            if (SkillCaculator.Chance(10))
            {
                TurnManager.instance.player.GainBuff("훈련");
            }
        }
        else if (groupSynergy.count >= 2)
        {
            // 정자세
            if(SkillCaculator.Chance(50))
            {
                TurnManager.instance.player.GainBuff("정자세");
            }
        }
    }

    // 지옥
    void ActHellSynergy(DeckSlot deckSlot)
    {
        GroupSynergy groupSynergy = Hand.instance.GetSynergy("지옥");

        if (groupSynergy == null) return;

        // 파멸
        if (groupSynergy.count >= 1 && groupSynergy.count < 5 && deckSlot.GetWeaponData.group.Contains("지옥"))
        {
            TurnManager.instance.player.GainBuff("파멸");
        }
    }

    // 모험가
    void ActAdventurerSynergy()
    {
        GroupSynergy groupSynergy = Hand.instance.GetSynergy("모험가");

        if (groupSynergy == null) return;

        /*
        //효과
        if (groupSynergy.count >= 4 )
        {
            TurnManager.instance.player.GainBuff("완전 적응", ()=> 
            {
                foreach(GroupSynergy i in Hand.instance.mostSynergy)
                {
                    if (i.name == "모험가") continue;
                    Hand.instance.GainSynergy(i.name);
                    Hand.instance.GainSynergy(i.name);
                }
            });
        }
        else 
        */
        if (groupSynergy.count >= 1)
        {
            TurnManager.instance.player.GainBuff("적응", ()=>
            {
                foreach (GroupSynergy i in Hand.instance.mostSynergy)
                {
                    if (i.name == "모험가") continue;
                    Hand.instance.GainSynergy(i.name);
                }
            });
        }

    }

    // 용병
    void ActMercenarySynergy()
    {
        GroupSynergy groupSynergy = Hand.instance.GetSynergy("용병");

        if (groupSynergy == null) return;

        // 파멸
        if (groupSynergy.count >= 4)
        {
            // 훈련
            if (SkillCaculator.Chance(10))
            {
                TurnManager.instance.player.GainBuff("갈무리");
            }
        }
        else if (groupSynergy.count >= 2)
        {
            ActSatisfy();
            TurnManager.instance.player.GainBuff("만족");
        }

        void ActSatisfy()
        {
            foreach(HandSlot i in Hand.instance.cards)
            {
                i.AddTempUpgrade(true, UpgradeType.Power, 1);
            }
        }
    }

    #endregion

    #region Buff

    // 버프 발동
    void EffectBuff(bool isPlayer,string buff, System.Action effect)
    {
        if(isPlayer)
        {
            if (TurnManager.instance.player.CheckBuff(buff))
            {
                TurnManager.instance.player.DecreaseBuff(buff);
                effect();
            }
        }
        else
        {
            if (TurnManager.instance.enemy.CheckBuff(buff))
            {
                TurnManager.instance.enemy.DecreaseBuff(buff);
                effect();
            }
        }
    }

    #endregion

    #region Mystery

    //신비 : 울창함
    void ActLushMystery()
    {
        // 매 턴 1 재생
    }

    void ActHardMystery()
    {

    }

    void ActWeekMystery()
    {

    }

    #endregion
}
