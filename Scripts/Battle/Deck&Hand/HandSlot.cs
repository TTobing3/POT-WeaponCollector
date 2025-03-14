using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSlot : MonoBehaviour
{
    public DeckSlot deckSlot;
    Tooltipable tooltipable;
    ParticleSystemRenderer upgradeParticle;

    public WeaponData GetWeaponData
    {
        get { return deckSlot.slot.weaponData == null ? null : deckSlot.slot.weaponData; }
    }

    Button button;
    Image itemImage;
    Material material;

    #region System
    void Awake()
    {
        button = GetComponent<Button>();
        tooltipable = GetComponent<Tooltipable>();
        itemImage = transform.GetChild(0).GetComponent<Image>();
        upgradeParticle = transform.GetChild(1).GetComponent<ParticleSystemRenderer>();

        material = itemImage.material;
    }
    void Start()
    {
        button.onClick.AddListener(RemoveButton);
    }
    #endregion
    #region Set
    public void Set(DeckSlot deckSlot)
    {
        this.deckSlot = deckSlot;
        tooltipable.data = $"Weapon:{GetWeaponData.name}";
        button.interactable = true;

        SetParticle();
        ChangeImage();
    }

    void ChangeImage()
    {
        if (deckSlot == null)
        {
            itemImage.gameObject.SetActive(false);
        }
        else
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = deckSlot.slot.sprite;
        }
    }
    #endregion

    #region Use
    public void RemoveButton()
    {
        if (TurnManager.instance.interectLock) return;

        Remove();
    }
    public void Use()
    {
        Remove();
    }
    public void Remove()
    {
        if (deckSlot == null)
        {
            tooltipable.data = "";
        }
        else
        {
            deckSlot.SelectSlot(false);
            Hand.instance.hands--;

            this.deckSlot = null;
            tooltipable.data = "";

            button.interactable = false;

            Hand.instance.UpdateGroupSynergy();
            UIManager.instance.UpdateCostUI();
        }

        ChangeImage();
        SetParticle();
    }

    public void SetParticle()
    {
        upgradeParticle.gameObject.SetActive(false);

        if (deckSlot != null && deckSlot.upgrade.count != 0)
        {
            var particleSystem = upgradeParticle.GetComponent<ParticleSystem>().main;
            var deckSlotparticleSystem = deckSlot.upgradeParticle.GetComponent<ParticleSystem>().main;
            particleSystem.duration = deckSlotparticleSystem.duration;
            particleSystem.startColor = deckSlotparticleSystem.startColor;

            upgradeParticle.gameObject.SetActive(true);
        }
    }

    #endregion
    #region TempUpgrade

    public void AddTempUpgrade(bool isTurn, UpgradeType type, int value)
    {
        if(deckSlot != null)
        {
            deckSlot.AddTempUpgrade(isTurn, type, value);
        }
    }

    public void ResetTempUpgrade(bool isTurn)
    {
        if (deckSlot != null)
        {
            deckSlot.ResetTempUpgrade(isTurn);
        }
    }

    #endregion
}
