using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MysteryManager : MonoBehaviour
{
    public GameObject mysteryScreen;

    public static MysteryManager instance;

    [Header("UI")]
    public MysteryItem[] optionItems;
    public Image optionBack, titleBack, panel;
    public TextMeshProUGUI optionTitleText, optionDesText, titleText;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        //Set();
    }

    public void Set()
    {
        mysteryScreen.SetActive(true);

        foreach(MysteryItem i in optionItems)
        {
            i.Set(DataManager.instance.AllMysteryDataList[Random.Range(0, DataManager.instance.AllMysteryDataList.Count)]);
        }

        //

        panel.DOFade(0.9f, 1);

        //

        titleBack.gameObject.SetActive(true);
        titleBack.color = Color.clear;
        titleText.color = Color.clear;
        titleBack.DOFade(0.8f, 1);
        titleText.DOColor(new Color(0.2f, 0.08f, 0.08f), 1);

        //

        optionBack.gameObject.SetActive(true);
        optionBack.color = Color.clear;

        //

        optionTitleText.color = new Color(1, 1, 1, 0);
        optionDesText.color = new Color(1, 1, 1, 0);

        //

        Deck.instance.MoveOutDeck();
    }

    public void Finish()
    {
        Fade.instance.FadeAction(()=> 
        {
            mysteryScreen.gameObject.SetActive(false);
            StageManager.instance.FinishRound();
            Deck.instance.MoveInDeck();
        });
    }

    public void itemSelect(int number)
    {
        DOTween.Kill(titleBack);
        DOTween.Kill(titleText);
        titleBack.DOFade(0, 0.1f);
        titleText.DOFade(0, 0.1f);

        //

        optionBack.DOFade(0.8f, 1);

        for (int i = 0; i<optionItems.Length; i++)
        {
            optionItems[i].Float(i == number);
            if (i == number)
            {
                var tmpTitle = optionItems[i].mysteryData.name;
                var tmpDes = optionItems[i].mysteryData.description;
                optionTitleText.DOFade(0, 0.2f).OnComplete(()=> {
                    optionTitleText.text = $"[ {tmpTitle} ]";
                    optionTitleText.DOFade(1, 0.2f);
                });

                optionDesText.DOFade(0, 0.2f).OnComplete(() => {
                    optionDesText.text = $"{tmpDes}";
                    optionDesText.DOFade(1, 0.2f);
                });

            }
        }

    }
}
