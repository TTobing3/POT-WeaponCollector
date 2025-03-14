using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerTooltip : MonoBehaviour
{
    RectTransform rect;



    [Header("ToolTip UI")]
    public TextMeshProUGUI alignmentText;
    public TextMeshProUGUI heartText, healthText, magicText;

    public List<Transform> trainLines;

    public TextMeshProUGUI descriptionText;

    [Header("Train")]
    public RectTransform typeAreaRect;
    public Transform trainContent;
    public GameObject trainPrefab;
    public List<TrainLine> trainPool;

    [Header("Buff")]

    public Transform buffContent;
    public GameObject buffPrefab;
    public List<BuffLine> buffPool;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        for (int i = 0; i < 16; i++)
        {
            var trainLine = Instantiate(trainPrefab, trainContent).GetComponent<TrainLine>();
            trainLine.gameObject.SetActive(false);

            trainPool.Add(trainLine);
        }
    }

    public void Set()
    {
        gameObject.SetActive(true);

        SetToolTip();
        SetBuff();
    }

    void SetToolTip()
    {
        ResetTooltip();

        alignmentText.text = $"{Deck.instance.GetMostAlignment(0)}/{Deck.instance.GetMostAlignment(1)}";

        heartText.text = TurnManager.instance.player.heart+"";

        healthText.text = TurnManager.instance.player.health + "";
        magicText.text = TurnManager.instance.player.magic + "";

        SetTrains();

    }

    void SetTrains()
    {
        foreach (TrainLine i in trainPool) i.gameObject.SetActive(false);

        var count = 0;

        for(int i = 0; i < 16; i++)
        {
            if (TurnManager.instance.player.trains[i] < 1) continue;
            trainPool[i].gameObject.SetActive(true);
            trainPool[i].Set(i);
            count++;
        }

        if (count == 0) typeAreaRect.gameObject.SetActive(false);
        else
        {
            typeAreaRect.sizeDelta = new Vector2(typeAreaRect.sizeDelta.x, (140 - 100) + (count * 100) + ( (count-1) * 5 ) );
        }
    }

    void SetBuff()
    {
        foreach (BuffLine i in buffPool) i.gameObject.SetActive(false);


        for (int i = 0; i<TurnManager.instance.player.buffList.Count; i++)
        {

            BuffLine buffLine = null;

            foreach (BuffLine j in buffPool)
            {
                if (j.gameObject.activeSelf) continue;
                buffLine = j;
                j.gameObject.SetActive(true);
            }

            if(buffLine == null)
            {
                buffLine = Instantiate(buffPrefab, buffContent).GetComponent<BuffLine>();
                buffPool.Add(buffLine);
            }

            buffLine.Set(TurnManager.instance.player.buffList[i].Split(':')[0]);
        }
    }

    void ResetTooltip()
    {


    }
}
