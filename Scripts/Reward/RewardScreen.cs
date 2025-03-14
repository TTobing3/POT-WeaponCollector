using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class TrainOption
{
    public WeaponType type;
    public Image typeImage, typeDisplay, background;
    public TextMeshProUGUI title, description;
}
public class RewardScreen : MonoBehaviour
{
    CanvasGroup rewardCanvaseGroup;

    [Header("Pocket")]
    public TextMeshProUGUI pocketTitle, pocketName;
    public Image pocketImage;

    [Header("Train")]
    public TextMeshProUGUI trainTitle;
    public TrainOption[] options;

    [Header("Weapon")]
    public TextMeshProUGUI weaponTitle;
    public Image weaponImage;

    [Header("Change")]
    public TextMeshProUGUI changeTitle;
    public Image changeImage;

    void Awake()
    {
        rewardCanvaseGroup = GetComponent<CanvasGroup>();
    }

    public void Fade(bool fadeIn, System.Action inCall = null, System.Action outCall = null)
    {
        if(fadeIn)
        {
            rewardCanvaseGroup.alpha = 0;
            DOTween.To(() => rewardCanvaseGroup.alpha, x => rewardCanvaseGroup.alpha = x, 1, 0.3f).SetEase(Ease.InCubic)
                .OnComplete(()=>
                {
                    if (inCall != null) inCall();
                });
        }
        else
        {
            rewardCanvaseGroup.alpha = 1;
            DOTween.To(() => rewardCanvaseGroup.alpha, x => rewardCanvaseGroup.alpha = x, 0, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if (outCall != null) outCall();
            });
        }

    }

    public void SetPocketScreen()
    {
        var stageLevel = StageManager.instance.curStage;

        pocketImage.gameObject.SetActive(true);
        pocketImage.color = Color.white;

        if (stageLevel > 5)
        {
            pocketName.text = "상처난 가죽 주머니";
        }
        else if( stageLevel <= 5 && stageLevel > 3 )
        {
            pocketName.text = "귀중한 주머니";
        }
        else if (stageLevel <= 3 && stageLevel > 1 )
        {
            pocketName.text = "보통 주머니";
        }
        else
        {
            pocketName.text = "낡은 주머니";
        }
    }

    public void SetTrainScreen(List<int> trainTypes)
    {
        var stageLevel = StageManager.instance.curStage;

        string trainLevel = "숙련";

        if (stageLevel > 2)
        {
            trainLevel = "고급 숙련";
        }

        for (int i = 0; i < 3; i++)
        {
            options[i].type = (WeaponType)trainTypes[i];
            options[i].typeImage.GetComponent<SlotImageAdapter>().ImageChange("Type", DataManager.instance.GetTypeText(trainTypes[i]),false);
            options[i].title.text = $"{DataManager.instance.GetTypeText(trainTypes[i])} {trainLevel}";
            options[i].description.text = $"{DataManager.instance.GetTypeText(trainTypes[i])} 숙련도 +{DataManager.instance.StageRewardTable[stageLevel].train}";
        }
    }

    public void SetWeaponScreen(string weapon)
    {
        weaponTitle.text = weapon + " 획득!";
        weaponImage.GetComponent<SlotImageAdapter>().ImageChange("Weapon",weapon,false);
        weaponImage.GetComponent<Tooltipable>().data = $"Weapon:{weapon}";
    }

    public void SetChangeScreen(string weapon)
    {
        changeTitle.text = weapon + "와(과) 교체할 무기를 선택하세요";
        changeImage.GetComponent<SlotImageAdapter>().ImageChange("Weapon", weapon, false);
        changeImage.GetComponent<Tooltipable>().data = $"Weapon:{weapon}";
    }

}
