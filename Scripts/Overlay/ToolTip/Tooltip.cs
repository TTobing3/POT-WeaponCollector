using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;

    RectTransform rect;
    CanvasGroup canvasGroup;

    public System.Action<string> ActionSet; //여기서 툴팁 종류별로 따로 등록해놓기

    public WeaponTooltip weaponTooltip;
    public CharacterTooltip characterTooltip;
    public GroupSynergyTooltip groupSynergyTooltip;
    public PlayerTooltip playerTooltip;
    public BuffTooltip buffTooltip;
    public MysteryTooltip mysteryTooltip;

    public Image fade;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);

        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Set(bool set, string data)
    {
        if (!set || data == null || data == "")
        {
            SetFade(false);
            gameObject.SetActive(false);
            return; 
        }

        ResetTooltip();

        var dataSplit = data.Split(":");


        SetFade(true);

        switch (dataSplit[0])
        {
            case "Weapon":
                gameObject.SetActive(true);
                weaponTooltip.Set(dataSplit[1]);
                break;

            case "Character":
                gameObject.SetActive(true);
                characterTooltip.Set(dataSplit[1]);
                break;

            case "GroupSynergy":
                gameObject.SetActive(true);
                groupSynergyTooltip.Set(dataSplit[1]);
                break;

            case "Player":
                gameObject.SetActive(true);
                playerTooltip.Set();
                break;

            case "Buff":
                gameObject.SetActive(true);
                buffTooltip.Set(dataSplit[1]);
                break;

            case "Mystery":
                gameObject.SetActive(true);
                mysteryTooltip.Set(dataSplit[1]);
                break;

            default:
                gameObject.SetActive(false); 
                return;
        }
    }

    public void Set(DeckSlot deckSlot)
    {
        if(deckSlot == null || deckSlot.GetWeaponData.number == -1)
        {
            gameObject.SetActive(false);
            return;
        }

        ResetTooltip();

        SetFade(true);

        gameObject.SetActive(true);
        weaponTooltip.Set(deckSlot);
    }

    void ResetTooltip()
    {
        OffTooltip();

        DOTween.Kill(rect);
        DOTween.Kill(canvasGroup);

        rect.localScale = Vector3.zero;
        rect.DOScale(Vector3.one, 0.2f);

        canvasGroup.alpha = 0;
        DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, 0.3f).SetEase(Ease.InCubic);

    }
    public void OffTooltip()
    {
        if (playerTooltip != null) playerTooltip.gameObject.SetActive(false);
        if (weaponTooltip != null) weaponTooltip.gameObject.SetActive(false);
        if (characterTooltip != null) characterTooltip.gameObject.SetActive(false);
        if (groupSynergyTooltip != null) groupSynergyTooltip.gameObject.SetActive(false);
        if (buffTooltip != null) buffTooltip.gameObject.SetActive(false);
        if (mysteryTooltip != null) mysteryTooltip.gameObject.SetActive(false);

        SetFade(false);
    }

    void SetFade(bool set)
    {
        DOTween.Kill(fade);

        if (set)
        {
            fade.gameObject.SetActive(true);

            fade.DOFade(0.8f, 0.5f);
        }
        else
        {
            fade.DOFade(0f, 0.5f).OnComplete(()=>
            {
                fade.gameObject.SetActive(false);
            });
        }
    }
}
