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
        // �� ��ŵ ��, �۵� �� ȿ���� ���� ������ ����

        #region ����

        EffectBuff(true, "����", () =>
        {
            TurnManager.instance.FinishPlayerSkill();
            next = null;

            return;
        });
        
        #endregion

        // �� ��ŵ ��, �۵� �� �� ȿ���� ���� ������ ����

        #region �Ҽ�
        //�ڿ�
        ActNatureSynergy();

        //����
        ActAdventurerSynergy();

        // �뺴
        ActMercenarySynergy();

        #endregion

        if (next != null)
            next();
    }

    // �� ���� ��
    public void FinishPlayerTurn(System.Action next)
    {
        EffectBuff(true, "����"     , () => {});
        EffectBuff(true, "���� ����", () => {});
        EffectBuff(true, "����", () => { });


        if (next != null)
            next();
    }

    // ���� ��� ���� ���� ��
    public float SetEffectRange(int[] range, System.Func<float> next)
    {
        float power = 0;

        if (next != null) power = next();

        EffectBuff(true, "���ڼ�", () =>
        {
            power = range[1];
        });

        return power;
    }

    // ���� ��� �� ( ���� ���� ���� )
    public void UseWeapon(DeckSlot deckSlot, System.Action next) // �÷��̾� ���� �ϳ� ���
    {
        #region �Ҽ�

        // ����
        ActHellSynergy(deckSlot);

        #endregion

        #region ����

        EffectBuff(true, "�ĸ�", () =>
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


        EffectBuff(true, "�Ʒ�", () => 
        {
            var type = (int)DataManager.instance.GetWeaponType(deckSlot.GetWeaponData.type);
            TurnManager.instance.player.AddTrain(type);
        });

        EffectBuff(true, "������", () =>
        {
            print("������ �۵�");

            var chance = Random.Range(0, 100);

            print($"Ȯ�� { chance }");

            if ( chance < 5)
            {
                GainMercenaryWeapon("������ ����ǰ");
            }
            else if (chance < 20)
            {
                GainMercenaryWeapon("��ġ�ִ� ����ǰ");
            }
            else if (chance < 50)
            {
                GainMercenaryWeapon("���� ����ǰ");
            }
            else
            {
                GainMercenaryWeapon("�������� ����ǰ");
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


    // ���� ���� ��
    public void FailedActEffect(DeckSlot deckSlot, System.Action next)
    {
        #region �Ҽ�
        #endregion

        #region ����
        #endregion

        if (next != null)
            next();
    }

    // ���� ���� ��
    public void SuccessActEffect(DeckSlot deckSlot, System.Action next)
    {
        #region �Ҽ�

        //����
        ActEmpireSynergy(deckSlot);

        #endregion

        #region ����

        #endregion

        if (next != null)
            next();
    }

    public void AttackEnemy(int damage, System.Action next)
    {
        EffectBuff(true, "����", () =>
        {
            TurnManager.instance.enemy.TakeDamage((int)(damage*1.5f));
            next = null;
        });

        EffectBuff(true ,"�е�", () =>
        {
            if (Random.Range(0, 100) < 50)
            {
                TurnManager.instance.enemy.GainBuff("����");
            }
            else
            {
                print($"�е� ����");
            }
        });

        if (next != null)
            next();
    }

    #endregion

    #region Enemy

    public void StartEnemyAct(System.Action next)
    {
        EffectBuff(false, "����", () =>
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

    // �ڿ�
    void ActNatureSynergy()
    {
        GroupSynergy groupSynergy = Hand.instance.GetSynergy("�ڿ�");

        if (groupSynergy == null) return;


        //ȿ��
        if (groupSynergy.count >= 5)
        {
            AddNatureWeapon("������ �ڿ�");
            AddNatureWeapon("������ �ڿ�");
        }
        else if (groupSynergy.count >= 4)
        {
            AddNatureWeapon("������ �ڿ�");
        }
        else if (groupSynergy.count >= 3)
        {
            AddNatureWeapon("�ڿ�");
            AddNatureWeapon("�ڿ�");
        }
        else if (groupSynergy.count >= 2)
        {
            AddNatureWeapon("�ڿ�");
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

    // ����
    void ActEmpireSynergy(DeckSlot deckSlot)
    {
        GroupSynergy groupSynergy = Hand.instance.GetSynergy("����");

        if (groupSynergy == null) return;


        //ȿ��
        if (groupSynergy.count >= 4)
        {
            TurnManager.instance.player.GainBuff("���ڼ�");
        }
        else if (groupSynergy.count >= 3)
        {
            // �Ʒ�
            if (SkillCaculator.Chance(10))
            {
                TurnManager.instance.player.GainBuff("�Ʒ�");
            }
        }
        else if (groupSynergy.count >= 2)
        {
            // ���ڼ�
            if(SkillCaculator.Chance(50))
            {
                TurnManager.instance.player.GainBuff("���ڼ�");
            }
        }
    }

    // ����
    void ActHellSynergy(DeckSlot deckSlot)
    {
        GroupSynergy groupSynergy = Hand.instance.GetSynergy("����");

        if (groupSynergy == null) return;

        // �ĸ�
        if (groupSynergy.count >= 1 && groupSynergy.count < 5 && deckSlot.GetWeaponData.group.Contains("����"))
        {
            TurnManager.instance.player.GainBuff("�ĸ�");
        }
    }

    // ���谡
    void ActAdventurerSynergy()
    {
        GroupSynergy groupSynergy = Hand.instance.GetSynergy("���谡");

        if (groupSynergy == null) return;

        /*
        //ȿ��
        if (groupSynergy.count >= 4 )
        {
            TurnManager.instance.player.GainBuff("���� ����", ()=> 
            {
                foreach(GroupSynergy i in Hand.instance.mostSynergy)
                {
                    if (i.name == "���谡") continue;
                    Hand.instance.GainSynergy(i.name);
                    Hand.instance.GainSynergy(i.name);
                }
            });
        }
        else 
        */
        if (groupSynergy.count >= 1)
        {
            TurnManager.instance.player.GainBuff("����", ()=>
            {
                foreach (GroupSynergy i in Hand.instance.mostSynergy)
                {
                    if (i.name == "���谡") continue;
                    Hand.instance.GainSynergy(i.name);
                }
            });
        }

    }

    // �뺴
    void ActMercenarySynergy()
    {
        GroupSynergy groupSynergy = Hand.instance.GetSynergy("�뺴");

        if (groupSynergy == null) return;

        // �ĸ�
        if (groupSynergy.count >= 4)
        {
            // �Ʒ�
            if (SkillCaculator.Chance(10))
            {
                TurnManager.instance.player.GainBuff("������");
            }
        }
        else if (groupSynergy.count >= 2)
        {
            ActSatisfy();
            TurnManager.instance.player.GainBuff("����");
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

    // ���� �ߵ�
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

    //�ź� : ��â��
    void ActLushMystery()
    {
        // �� �� 1 ���
    }

    void ActHardMystery()
    {

    }

    void ActWeekMystery()
    {

    }

    #endregion
}
