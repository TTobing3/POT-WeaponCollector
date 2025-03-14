using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class WeaponTooltip : MonoBehaviour
{
    RectTransform rect;

    [Header("ToolTip UI")]
    public SlotImageAdapter itemImageAdapter;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText,  groupText, rankText, alignmentText;
    public SlotImageAdapter[] groupImageAdapter;
    public TextMeshProUGUI healthText, manaText;
    public TextMeshProUGUI skillText;
    public TextMeshProUGUI descriptionText;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Set(string weapon)
    {
        gameObject.SetActive(true);

        var weaponData = DataManager.instance.AllWeaponDatas[weapon];

        SetToolTip(weaponData);
    }

    public void Set(DeckSlot deckSlot)
    {
        gameObject.SetActive(true);
        SetToolTip(deckSlot);
    }

    void SetToolTip(WeaponData weaponData)
    {
        ResetTooltip();

        // �̸�
        nameText.text = weaponData.name;

        // �̹���
        if (DataManager.instance.AllWeaponDataList.Contains(weaponData))
        {
            itemImageAdapter.ImageChange("Weapon", weaponData.name, false);
        }
        else
        {
            itemImageAdapter.ImageChange("Weapon", "����", false);
        }

        // Ÿ��
        string type = weaponData.type;

        typeText.text = type.Trim('/');

        // �Ҽ�

        if (weaponData.group.Length == 2)
        {
            groupText.text = $"{weaponData.group[0]}/{weaponData.group[1]}";
            groupText.fontSize = 48;

            groupImageAdapter[0].gameObject.SetActive(true);
            groupImageAdapter[1].gameObject.SetActive(true);

            groupImageAdapter[0].ImageChange("GroupIcon", weaponData.group[0], false);
            groupImageAdapter[1].ImageChange("GroupIcon", weaponData.group[1], false);
        }
        else
        {
            groupText.text = $"{weaponData.group[0]}";
            groupText.fontSize = 48;

            groupImageAdapter[0].gameObject.SetActive(true);

            groupImageAdapter[0].ImageChange("GroupIcon", weaponData.group[0], false);
        }



        // ���

        rankText.text = weaponData.rank;

        // ����

        string alignment = "";

        foreach (string i in weaponData.alignment) alignment += i + "/";

        alignmentText.text = alignment.Trim('/');

        // �ڽ�Ʈ

        healthText.text = $"x{weaponData.health}";
        manaText.text = $"x{weaponData.magic}";

        // ��ų

        foreach (string i in weaponData.skills)
        {
            var skillData = DataManager.instance.AllSkillDatas[i];
            skillText.text += $"<{i}>\n";

            var placeHolder = "{" + skillData.effectType + "}";

            var description = $"{skillData.chance}%�� Ȯ���� ";

            description += skillData.description
                .Replace( placeHolder, GetReplaceValue(skillData, weaponData.power));

            skillText.text += $"{description}\n";
        }



        //

        descriptionText.text = weaponData.backstory;

        //

        StartCoroutine(CoResize());

    }


    void SetToolTip(DeckSlot deckSlot)
    {
        ResetTooltip();

        var weaponData = deckSlot.GetWeaponData;

        // �̸�
        nameText.text = weaponData.name;

        if (deckSlot.upgrade.count != 0) nameText.text += $"+{deckSlot.upgrade.count}";

        // �̹���
        if (DataManager.instance.AllWeaponDataList.Contains(weaponData))
        {
            itemImageAdapter.ImageChange("Weapon", weaponData.name, false);
        }
        else
        {
            itemImageAdapter.ImageChange("Weapon", "����", false);
        }

        // Ÿ��
        string type = weaponData.type;

        typeText.text = type.Trim('/');

        // �Ҽ�

        if (weaponData.group.Length == 2)
        {
            groupText.text = $"{weaponData.group[0]}/{weaponData.group[1]}";
            groupText.fontSize = 48;

            groupImageAdapter[0].gameObject.SetActive(true);
            groupImageAdapter[1].gameObject.SetActive(true);

            groupImageAdapter[0].ImageChange("GroupIcon", weaponData.group[0], false);
            groupImageAdapter[1].ImageChange("GroupIcon", weaponData.group[1], false);
        }
        else
        {
            groupText.text = $"{weaponData.group[0]}";
            groupText.fontSize = 48;

            groupImageAdapter[0].gameObject.SetActive(true);

            groupImageAdapter[0].ImageChange("GroupIcon", weaponData.group[0], false);
        }

        // ���

        rankText.text = weaponData.rank;

        // ����

        string alignment = "";

        foreach (string i in weaponData.alignment) alignment += i + "/";

        alignmentText.text = alignment.Trim('/');

        // �ڽ�Ʈ

        healthText.text = $"x{deckSlot.GetWeaponHealth}";
        manaText.text = $"x{deckSlot.GetWeaponMagic}";

        // ��ų

        foreach (string i in weaponData.skills)
        {
            var skillData = DataManager.instance.AllSkillDatas[i];
            skillText.text += $"<{i}>\n";

            var effect = DataManager.instance.AllSkillDatas[i].effect.Split(":");

            var placeHolder = "{" + effect[0] + "}";

            var description = $"{skillData.chance + deckSlot.upgrade.luck + deckSlot.turnUpgrade.luck + deckSlot.roundUpgrade.luck}%�� Ȯ���� ";

            description += skillData.description
                .Replace(placeHolder, GetReplaceValue(skillData, deckSlot.GetWeaponPower))
                .Trim();

            skillText.text += $"{description}\n";
        }

        //

        descriptionText.text = weaponData.backstory;

        //

        StartCoroutine(CoResize());

    }
    
    string GetReplaceValue(SkillData skillData, float weaponPower)
    {
        if (skillData.effect.Contains('~'))
        {
            return 
            SkillCaculator.Caculate(skillData.effectType, weaponPower, skillData.effectRange[0]) +
            "~" +
            SkillCaculator.Caculate(skillData.effectType, weaponPower, skillData.effectRange[1]);
        }
        else
        {
            return SkillCaculator.Caculate(skillData.effectType, weaponPower, skillData.effectRange[0]);
        }
    }

    void ResetTooltip()
    {

        // �̸�
        nameText.text = "";

        // Ÿ��
        typeText.text = "";

        // �Ҽ�
        groupText.text = "";

        groupImageAdapter[0].gameObject.SetActive(false);
        groupImageAdapter[1].gameObject.SetActive(false);

        // ���
        rankText.text = "";

        // ����
        alignmentText.text = "";

        // �ڽ�Ʈ

        healthText.text = "";
        manaText.text = "";

        // ��ų
        skillText.text = "";

        // ����

        descriptionText.text = "";

    }

    IEnumerator CoResize()
    {
        yield return null;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 580 + skillText.GetComponent<RectTransform>().sizeDelta.y + 40);
    }
}
