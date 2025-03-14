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
            pocketName.text = "��ó�� ���� �ָӴ�";
        }
        else if( stageLevel <= 5 && stageLevel > 3 )
        {
            pocketName.text = "������ �ָӴ�";
        }
        else if (stageLevel <= 3 && stageLevel > 1 )
        {
            pocketName.text = "���� �ָӴ�";
        }
        else
        {
            pocketName.text = "���� �ָӴ�";
        }
    }

    public void SetTrainScreen(List<int> trainTypes)
    {
        var stageLevel = StageManager.instance.curStage;

        string trainLevel = "����";

        if (stageLevel > 2)
        {
            trainLevel = "��� ����";
        }

        for (int i = 0; i < 3; i++)
        {
            options[i].type = (WeaponType)trainTypes[i];
            options[i].typeImage.GetComponent<SlotImageAdapter>().ImageChange("Type", DataManager.instance.GetTypeText(trainTypes[i]),false);
            options[i].title.text = $"{DataManager.instance.GetTypeText(trainTypes[i])} {trainLevel}";
            options[i].description.text = $"{DataManager.instance.GetTypeText(trainTypes[i])} ���õ� +{DataManager.instance.StageRewardTable[stageLevel].train}";
        }
    }

    public void SetWeaponScreen(string weapon)
    {
        weaponTitle.text = weapon + " ȹ��!";
        weaponImage.GetComponent<SlotImageAdapter>().ImageChange("Weapon",weapon,false);
        weaponImage.GetComponent<Tooltipable>().data = $"Weapon:{weapon}";
    }

    public void SetChangeScreen(string weapon)
    {
        changeTitle.text = weapon + "��(��) ��ü�� ���⸦ �����ϼ���";
        changeImage.GetComponent<SlotImageAdapter>().ImageChange("Weapon", weapon, false);
        changeImage.GetComponent<Tooltipable>().data = $"Weapon:{weapon}";
    }

}
