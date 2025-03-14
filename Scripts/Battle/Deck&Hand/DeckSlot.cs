using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class Upgrade
{
    public int count = 0, power = 0, health = 0, magic = 0, luck = 0;
}

public enum UpgradeType { Power, Health, Magic, Luck };

public class DeckSlot : MonoBehaviour
{
    public Slot slot;
    public Tooltipable tooltipable;
    public Upgrade upgrade = new Upgrade(), turnUpgrade = new Upgrade(), roundUpgrade = new Upgrade();

    public List<string> runes = new List<string>();
    public bool select = false;

    public Image itemImage, effectImage;
    public ParticleSystemRenderer upgradeParticle;

    Button button;
    SlotImageAdapter slotImageAdapter;
   

    #region Property

    public WeaponData GetWeaponData
    {
        get { return slot.weaponData; }
    }
    public float GetWeaponPower
    {
        get
        {
            float power = GetWeaponData.power;

            power = (power + upgrade.power + turnUpgrade.power + roundUpgrade.power) * TurnManager.instance.player.GetTrainLevel(GetWeaponData.type).power;

            return power;
        }
    }
    public int GetWeaponHealth
    {
        get
        {
            var health = GetWeaponData.health - upgrade.health - turnUpgrade.health - roundUpgrade.health;
            return health < 1 ? 0 : health;
        }
    }
    public int GetWeaponMagic
    {
        get
        {
            var magic = GetWeaponData.magic - upgrade.magic - turnUpgrade.magic - roundUpgrade.magic;
            return magic < 1 ? 0 : magic;
        }
    }
    public HandSlot FindConnectedHandSlot
    {
        get
        {
            return Hand.instance.cards.Find(x => x.deckSlot == this);
        }
    }

    #endregion

    #region System
    private void Awake()
    {
        slot = GetComponent<Slot>();
        button = GetComponent<Button>();
        tooltipable = GetComponent<Tooltipable>();
        itemImage = transform.GetChild(0).GetComponent<Image>();
        effectImage = transform.GetChild(1).GetComponent<Image>();
        upgradeParticle = transform.GetChild(2).GetComponent<ParticleSystemRenderer>();
        slotImageAdapter = transform.GetChild(0).GetComponent<SlotImageAdapter>();

        button.onClick.AddListener(ClickSlot);
    }

    void Start()
    {
        button.onClick.AddListener(ChangeSlotByNewWeapon);
    }

    #endregion

    #region UI

    public void ChangeParticleOrder(int order)
    {
        upgradeParticle.sortingOrder = order;
    }

    #endregion

    #region Set
    public void SetSlot(string weapon)
    {
        if(weapon == "")
        {
            DropSlot();
        }
        else
        {
            itemImage.gameObject.SetActive(true);
            slot.Set(weapon, itemImage);
            button.interactable = true;
            tooltipable.data = $"Weapon:{weapon}";
            slotImageAdapter.ImageChange("Weapon", weapon, false);
            SetUpgradeEffect();
        }
    }

    public void SwapSlot(DeckSlot otherSlot)
    {
        // ���׷��̵� ��ȯ
        var tmpUpgrade = upgrade;
        upgrade = otherSlot.upgrade;
        otherSlot.upgrade = tmpUpgrade;

        // �� ��ȯ
        List<string> tempRunes = new List<string>(runes);
        runes = new List<string>(otherSlot.runes);
        otherSlot.runes = new List<string>(tempRunes);

        // ���� ���� ��ȯ
        bool tempSelect = select;
        select = otherSlot.select;
        otherSlot.select = tempSelect;

        // WeaponData ��ȯ [������] (���� ���� �� �̹��� �� �� ���ϴ� �� ��ȯ)
        string tmpWeapon = GetWeaponData.name;
        SetSlot(otherSlot.GetWeaponData.name);
        otherSlot.SetSlot(tmpWeapon);

    }

    public void DropSlot()
    {
        slot.Set();

        tooltipable.data = "";

        upgrade = new Upgrade();

        select = false;
        button.interactable = false;
        runes = new List<string>();
        slotImageAdapter.GetComponent<Image>().color = new Color(1, 1, 1, 1f);

        itemImage.gameObject.SetActive(false);

        SetUpgradeEffect();
    }

    public void Erase()
    {
        var handSlot = FindConnectedHandSlot;

        if(handSlot != null)
        {
            print("�� ���Կ� ����� �ڵ彽�� ������ ���� ��!");
            handSlot.Remove();
        }
        print("�� ���� ������ ���� ��!");

        DropSlot();

    }
    #endregion

    #region Use

    // ��ȯ
    public void ChangeSlotByNewWeapon()
    {
        if (Deck.instance.changing && StageManager.instance.rewardManager.selectedWeapon != "")
        {
            StageManager.instance.rewardManager.SelectChangeTargetSlot(this);
        }
    }

    // ����
    public void ClickSlot()
    {
        if(select && !TurnManager.instance.interectLock)
        {
            var targetHandSlot = FindConnectedHandSlot;
            targetHandSlot.Remove();
        }
        else
        {
            SelectSlot(true, Hand.instance.SearchEmptyHandSlot());
        }
    }

    public void SelectSlot(bool isSelect, HandSlot handSlot = null)
    {

        // �÷��̾� �غ� ��, ��ȣ�ۿ� �� �ƴ� ���
        if (TurnManager.instance.turn[0] == 0 || TurnManager.instance.interectLock) return;

        // ���Ⱑ ���ų�, �ֹ����� �ƴ� ���
        if (slot.weaponData.number == -1 || GetWeaponData.type.Contains("�ֹ���")) return;

        if (isSelect)
        {
            if (!CheckGuitar()) return;

            EventManager.instance.EquipWeapon(() =>
            {
                handSlot.Set(this);

                select = true;
                slotImageAdapter.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                UIManager.instance.UpdateCostUI();
                Hand.instance.AddCard();
            });

        }
        else
        {
            select = false;
            slotImageAdapter.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        }
    }

    public bool CheckGuitar()
    {
        //���� �Ǿ��ְų�
        if (select) return false;

        //�ڵ� �� ���� ���� ���
        if (Hand.instance.SearchEmptyHandSlot() == null) return false;

        // ü�� ���� �ڽ�Ʈ ���
        if (Hand.instance.healthCost + GetWeaponHealth > TurnManager.instance.player.health) return false;
        if (Hand.instance.magicCost + GetWeaponMagic > TurnManager.instance.player.magic) return false;

        return true;
    }

    // ���׷��̵� �õ�
    public void TryUpgrade(WeaponData weaponData)
    {
        upgrade.count += 1;

        foreach(string i in weaponData.skills)
        {
            var skillData = DataManager.instance.AllSkillDatas[i];

            if (Random.Range(0, 100) < skillData.chance) // Ȯ��
                Upgrade(true, skillData);
            else
                Upgrade(false, skillData);
        }
    }

    // ���׷��̵�
    void Upgrade(bool isSuccess, SkillData skillData)
    {
        if(isSuccess)
        {
            print($"�ֹ��� ����! {skillData.effectType}");

            ActUpgradeSuccessEffect();
            SetUpgradeEffect();

            switch (skillData.effectType)
            {
                case "����": upgrade.power += (int)skillData.effectPower; break;
                case "ü��": upgrade.health += (int)skillData.effectPower; break;
                case "����": upgrade.magic += (int)skillData.effectPower; break;
                case "���": upgrade.luck += (int)skillData.effectPower; break;
            }
        }
        else
        {
            print($"�ֹ��� ����...");
        }
    }

    void ActUpgradeSuccessEffect()
    {
        effectImage.gameObject.SetActive(true);

        DOTween.Kill(effectImage);

        effectImage.color = Color.white;
        effectImage.DOFade(0, 1).OnComplete(() => effectImage.gameObject.SetActive(false));
    }

    void SetUpgradeEffect()
    {
        upgradeParticle.gameObject.SetActive(false);
        
        if (upgrade.count > 0)
        {
            var particleSystem = upgradeParticle.GetComponent<ParticleSystem>().main;

            var value = 10 - upgrade.count * 2;
            particleSystem.duration = ( value < 1 ? 1 : value) * 0.1f;

            if (upgrade.count > 4)
            {
                bool Max(int target, int n1, int n2)
                {
                    return target >= n1 && target >= n2;
                }

                if (Max(upgrade.power, upgrade.health, upgrade.magic))
                {
                    particleSystem.startColor = new Color(1, 0.8f, 0);
                }
                else if (Max(upgrade.magic, upgrade.power, upgrade.health))
                {
                    particleSystem.startColor = new Color(0.2f, 0, 0.4f);
                }
                else if (Max(upgrade.health, upgrade.magic, upgrade.power))
                {
                    particleSystem.startColor = new Color(0.5f, 0, 0);
                }
            }

            // ������ �ڵ嵵 ����Ʈ ����
            if(FindConnectedHandSlot != null)
            {
                FindConnectedHandSlot.SetParticle();
            }

            upgradeParticle.gameObject.SetActive(true);
        }
    }

    #endregion

    #region TempUpgrade

    public void AddTempUpgrade( bool isTurn, UpgradeType type, int value )
    {
        if(isTurn)
        {
            switch (type)
            {
                case UpgradeType.Power:
                    turnUpgrade.power += value;
                    break;

                case UpgradeType.Health:
                    turnUpgrade.health += value;
                    break;

                case UpgradeType.Magic:
                    turnUpgrade.magic += value;
                    break;

                case UpgradeType.Luck:
                    turnUpgrade.luck += value;
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case UpgradeType.Power:
                    roundUpgrade.power += value;
                    break;

                case UpgradeType.Health:
                    roundUpgrade.health += value;
                    break;

                case UpgradeType.Magic:
                    roundUpgrade.magic += value;
                    break;

                case UpgradeType.Luck:
                    roundUpgrade.luck += value;
                    break;
            }
        }
    }

    public void ResetTempUpgrade( bool isTurn )
    {
        if(isTurn)
        {
            turnUpgrade = new Upgrade();
        }
        else
        {
            roundUpgrade = new Upgrade();
        }
    }

    #endregion

}
