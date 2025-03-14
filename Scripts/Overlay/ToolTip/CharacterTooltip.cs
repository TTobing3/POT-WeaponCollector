using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterTooltip : MonoBehaviour
{
    RectTransform rect;

    [Header("ToolTip UI")]
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI groupText, alignmentText;
    public SlotImageAdapter[] groupImageAdapter;
    public SlotImageAdapter characterImageAdapter;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText, armorText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI curSkillText, nextSkillText;

    [Header("Buff")]

    public Transform buffContent;
    public GameObject buffPrefab;
    public List<BuffLine> buffPool;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    public void Set(string enemy)
    {
        gameObject.SetActive(true);

        var enemyData = DataManager.instance.AllEnemyDatas[enemy];

        SetToolTip(enemyData);
    }
    void SetToolTip(EnemyData enemy)
    {
        ResetTooltip();

        // Ÿ��
        typeText.text = "��";

        // �̹���
        characterImageAdapter.ImageChange("Character", enemy.name, false);

        // �̸�
        nameText.text = enemy.name;

        // �Ҽ�

        if (enemy.group.Length == 2)
        {
            groupText.text = $"{enemy.group[0]}/{enemy.group[1]}";
            groupText.fontSize = 48;

            groupImageAdapter[0].gameObject.SetActive(true);
            groupImageAdapter[1].gameObject.SetActive(true);

            groupImageAdapter[0].ImageChange("GroupIcon", enemy.group[0], false);
            groupImageAdapter[1].ImageChange("GroupIcon", enemy.group[1], false);
        }
        else
        {
            groupText.text = $"{enemy.group[0]}";
            groupText.fontSize = 48;

            groupImageAdapter[0].gameObject.SetActive(true);

            groupImageAdapter[0].ImageChange("GroupIcon", enemy.group[0], false);
        }

        // ����

        string alignment = "";

        foreach (string i in enemy.alignment) alignment += i + "/";

        alignmentText.text = alignment.Trim('/');

        // �ڽ�Ʈ

        healthText.text = $"{enemy.health}";
        armorText.text = $"{enemy.armor}";

        // ����

        descriptionText.text = enemy.description;

        // ��ų

        var curIndex = TurnManager.instance.enemy.patternIndex;

        var curPattern = enemy.pattern[(curIndex + 1) % enemy.pattern.Count];
        var nextPattern = enemy.pattern[(curIndex + 2) % enemy.pattern.Count];

        var curSkillDescription = DataManager.instance.AllEnemySkillDatas[curPattern].description;
        var nextSkillDescription = DataManager.instance.AllEnemySkillDatas[nextPattern].description;

        foreach (string j in DataManager.instance.AllEnemySkillDatas[curPattern].effects)
        {
            var placeHolder = "{" + j.Split(":")[0] + "}";
            curSkillDescription = curSkillDescription
                .Replace(placeHolder, // {����} �� ����
                j.Split(":")[1]);
        }
        foreach (string j in DataManager.instance.AllEnemySkillDatas[nextPattern].effects)
        {
            var placeHolder = "{" + j.Split(":")[0] + "}";
            nextSkillDescription = nextSkillDescription
                .Replace(placeHolder, // {����} �� ����
                j.Split(":")[1]);
        }

        curSkillText.text += $"<{curPattern}>\n{curSkillDescription}\n";
        nextSkillText.text += $"<{nextPattern}>\n{nextSkillDescription}\n";

        //����
        SetBuff();
    }

    void SetBuff()
    {

        foreach (BuffLine i in buffPool) i.gameObject.SetActive(false);


        for (int i = 0; i < TurnManager.instance.enemy.buffList.Count; i++)
        {

            BuffLine buffLine = null;

            foreach (BuffLine j in buffPool)
            {
                if (j.gameObject.activeSelf) continue;
                buffLine = j;
                j.gameObject.SetActive(true);
            }

            if (buffLine == null)
            {
                buffLine = Instantiate(buffPrefab, buffContent).GetComponent<BuffLine>();
                buffPool.Add(buffLine);
            }

            buffLine.Set(TurnManager.instance.enemy.buffList[i].Split(':')[0]);
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


        // ����
        alignmentText.text = "";

        // �ڽ�Ʈ

        healthText.text = "";
        armorText.text = "";

        // ��ų
        curSkillText.text = "";
        nextSkillText.text = "";

        // ����

        descriptionText.text = "";

    }
}
