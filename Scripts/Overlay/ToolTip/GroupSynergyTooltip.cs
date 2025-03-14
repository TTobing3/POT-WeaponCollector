using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GroupSynergyTooltip : MonoBehaviour
{
    RectTransform rect;

    [Header("Tooltip UI")]
    public SlotImageAdapter groupIconImageAdapter;
    public TextMeshProUGUI titleText, descriptionText;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Set(string groupSynergyData)
    {
        gameObject.SetActive(true);

        var groupSynergy = DataManager.instance.AllGroupSynergyDatas[groupSynergyData];

        SetToolTip(groupSynergy);

        StartCoroutine(CoResize());
    }

    void SetToolTip(GroupSynergyData groupSynergy)
    {
        groupIconImageAdapter.ImageChange("GroupIcon", groupSynergy.name, false);

        titleText.text = groupSynergy.name;

        string description = "";

        description += $" {groupSynergy.description}\n";

        for(int i = 0; i<groupSynergy.effects.Count; i++)
        {
            if (groupSynergy.effects[i] == "¾øÀ½") continue;

            description += $"[{i}] {groupSynergy.effects[i]}\n";
        }

        descriptionText.text = description;
    }

    void ResetToolTip()
    {

    }


    IEnumerator CoResize()
    {
        yield return null;
    }
}
