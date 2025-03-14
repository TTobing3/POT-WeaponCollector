using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrainLine : MonoBehaviour
{
    [Header("UI")]
    public SlotImageAdapter iconAdapter;
    public TextMeshProUGUI levelText, typeText, trainText;

    public void Set(int type)
    {
        string stype = DataManager.instance.GetTypeText(type);
        iconAdapter.ImageChange("Type", stype, false);
        levelText.text = $"[{TurnManager.instance.player.GetTrainLevel(stype).name}]";
        typeText.text = $"{stype} ���õ�";
        trainText.text = TurnManager.instance.player.trains[type]+"";
    }
}
