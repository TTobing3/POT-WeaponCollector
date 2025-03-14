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
        // 업그레이드 교환
        var tmpUpgrade = upgrade;
        upgrade = otherSlot.upgrade;
        otherSlot.upgrade = tmpUpgrade;

        // 룬 교환
        List<string> tempRunes = new List<string>(runes);
        runes = new List<string>(otherSlot.runes);
        otherSlot.runes = new List<string>(tempRunes);

        // 선택 상태 교환
        bool tempSelect = select;
        select = otherSlot.select;
        otherSlot.select = tempSelect;

        // WeaponData 교환 [마지막] (무기 정보 및 이미지 등 안 변하는 값 교환)
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
            print("덱 슬롯에 연결된 핸드슬롯 아이템 제거 중!");
            handSlot.Remove();
        }
        print("덱 슬롯 아이템 제거 중!");

        DropSlot();

    }
    #endregion

    #region Use

    // 교환
    public void ChangeSlotByNewWeapon()
    {
        if (Deck.instance.changing && StageManager.instance.rewardManager.selectedWeapon != "")
        {
            StageManager.instance.rewardManager.SelectChangeTargetSlot(this);
        }
    }

    // 장착
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

        // 플레이어 준비 턴, 상호작용 락 아닐 경우
        if (TurnManager.instance.turn[0] == 0 || TurnManager.instance.interectLock) return;

        // 무기가 없거나, 주문서가 아닌 경우
        if (slot.weaponData.number == -1 || GetWeaponData.type.Contains("주문서")) return;

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
        //선택 되어있거나
        if (select) return false;

        //핸드 빈 곳이 없을 경우
        if (Hand.instance.SearchEmptyHandSlot() == null) return false;

        // 체력 마력 코스트 계산
        if (Hand.instance.healthCost + GetWeaponHealth > TurnManager.instance.player.health) return false;
        if (Hand.instance.magicCost + GetWeaponMagic > TurnManager.instance.player.magic) return false;

        return true;
    }

    // 업그레이도 시도
    public void TryUpgrade(WeaponData weaponData)
    {
        upgrade.count += 1;

        foreach(string i in weaponData.skills)
        {
            var skillData = DataManager.instance.AllSkillDatas[i];

            if (Random.Range(0, 100) < skillData.chance) // 확률
                Upgrade(true, skillData);
            else
                Upgrade(false, skillData);
        }
    }

    // 업그레이드
    void Upgrade(bool isSuccess, SkillData skillData)
    {
        if(isSuccess)
        {
            print($"주문서 성공! {skillData.effectType}");

            ActUpgradeSuccessEffect();
            SetUpgradeEffect();

            switch (skillData.effectType)
            {
                case "위력": upgrade.power += (int)skillData.effectPower; break;
                case "체력": upgrade.health += (int)skillData.effectPower; break;
                case "마력": upgrade.magic += (int)skillData.effectPower; break;
                case "행운": upgrade.luck += (int)skillData.effectPower; break;
            }
        }
        else
        {
            print($"주문서 실패...");
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

            // 장착된 핸드도 이펙트 변경
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
