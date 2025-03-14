using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

public class StoreManager : MonoBehaviour
{
    public GameObject traderScreen;

    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI[] nameTexts;
    public Image[] images;
    public Button[] buttons;
    public TextMeshProUGUI[] priceTexts;
    public Button leaveButton;
    public CanvasGroup canvasGroup;

    [Header("-")]
    public RoundData roundData;
    public string[] item = new string[3] { "", "", "" };
    public string[] itemType = new string[3] { "", "", "" };
    public int[] itemPrice = new int[3];
    public SlotImageAdapter imageAdapter, randomSelecter, npcAdapter;
    bool[] productSet = new bool[] { false, false, false };

    void Start()
    {
        buttons[0].onClick.AddListener(() => { SelectOption(0); });
        buttons[1].onClick.AddListener(() => { SelectOption(1); });
        buttons[2].onClick.AddListener(() => { SelectOption(2); });

        leaveButton.onClick.AddListener(FinishStoreRound);
        //Set("방랑 마법사");
    }

    public void Fade(bool fadeIn, System.Action inCall = null, System.Action outCall = null)
    {
        if (fadeIn)
        {
            canvasGroup.alpha = 0;
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, 0.3f).SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    if (inCall != null) inCall();
                });
        }
        else
        {
            canvasGroup.alpha = 1;
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if (outCall != null) outCall();
            });
        }

    }

    public void Set()
    {
        traderScreen.gameObject.SetActive(true);
        leaveButton.interactable = false;

        roundData = StageManager.instance.roundData;
        titleText.text = roundData.name;

        npcAdapter.ImageChange("Character", roundData.target, false);

        productSet = new bool[] { false, false, false };
        itemType = new string[3] { "", "", "" };
        itemPrice = new int[3];
        foreach (TextMeshProUGUI i in nameTexts) i.text = "???";

        for(int i = 0; i<3; i++)
        {
            nameTexts[i].text = "???";
            images[i].color = new Color(1, 1, 1, 1);
            images[i].gameObject.SetActive(false);
            buttons[i].interactable = false;
            priceTexts[i].text = "?G";
        }

        Fade(true, () =>
        {
            for (int i = 0; i < 3; i++) images[i].gameObject.SetActive(true);

            StartCoroutine(SetRandomImage(0));
            StartCoroutine(SetRandomImage(1));
            StartCoroutine(SetRandomImage(2));

            StartCoroutine(SetItem(0, 0.3f));
            StartCoroutine(SetItem(1, 0.6f));
            StartCoroutine(SetItem(2, 0.9f));
        });

        Deck.instance.GetComponent<RectTransform>().DOAnchorPosY(-800, 0.2f).OnComplete(() => {
            Deck.instance.ChangeParent(traderScreen.transform);
            Deck.instance.GetComponent<RectTransform>().DOAnchorPosY(-480, 0.8f);
        });
    }

    IEnumerator SetItem(int number, float delay)
    {
        yield return new WaitForSeconds(delay);

        productSet[number] = true;

        string randomItem = "";

        if (roundData.target == "방랑 상인")
        {
            randomItem = SetTraderItem(number);
        }
        else if (roundData.target == "방랑 마법사")
        {
            randomItem = SetWizardItem(number);
        }

        item[number] = randomItem;

        yield return null;

        images[number].sprite = imageAdapter.GetComponent<SpriteRenderer>().sprite;
        nameTexts[number].text = randomItem;

        var rank = DataManager.instance.AllWeaponDatas[item[number]].rank;
        var price = Random.Range(DataManager.instance.StoreTable[rank][0], DataManager.instance.StoreTable[rank][1]);
        priceTexts[number].text = price+"G";
        itemPrice[number] = price;

        buttons[number].interactable = true;

        if (productSet[0] && productSet[1] && productSet[2])
            leaveButton.interactable = true;

    }

    string SetTraderItem(int number)
    {
        string randomItem = "";
        string[] exceptionRank = new string[] { "자연", "안정", "불안정", "소실" };

        if (number == 0)
        {
            var mostGroup = DataManager.instance.AllWeaponDataByGroup[Deck.instance.GetMostGroup()];

            do
            {
                randomItem = mostGroup[Random.Range(0, mostGroup.Count)];
            }
            while (exceptionRank.Contains( DataManager.instance.AllWeaponDatas[randomItem].rank ));
        }
        else if (number == 1)
        {
            var mostType = DataManager.instance.AllWeaponDataByType[Deck.instance.GetMostType()];

            do
            {
                randomItem = mostType[Random.Range(0, mostType.Count)];
            }
            while (exceptionRank.Contains(DataManager.instance.AllWeaponDatas[randomItem].rank));
        }
        else if (number == 2)
        {
            randomItem = "안정적인 주문서";
        }

        imageAdapter.ImageChange("Weapon", randomItem);

        buttons[number].GetComponent<Tooltipable>().data = $"Weapon:{randomItem}";

        return randomItem;
    }

    string SetWizardItem(int number)
    {
        string randomItem = "";
        string[] exceptionRank = new string[] { "안정", "불안정", "소실", "신비" };

        if (number == 0)
        {
            var mostRank = DataManager.instance.AllWeaponDataByRank[exceptionRank[Random.Range(0,4)]];
            randomItem = mostRank[Random.Range(0, mostRank.Count)];
        }
        else if (number == 1)
        {
            var mostRank = DataManager.instance.AllWeaponDataByRank[exceptionRank[Random.Range(0, 4)]];
            randomItem = mostRank[Random.Range(0, mostRank.Count)];
        }
        else if (number == 2)
        {
            var mostRank = DataManager.instance.AllWeaponDataByRank[exceptionRank[Random.Range(0, 4)]];
            randomItem = mostRank[Random.Range(0, mostRank.Count)];

        }

        imageAdapter.ImageChange("Weapon", randomItem);


        buttons[number].GetComponent<Tooltipable>().data = $"Weapon:{randomItem}";

        return randomItem;
    }

    IEnumerator SetRandomImage(int number)
    {
        if (productSet[number]) yield break;

        var randomWeapon = DataManager.instance.AllWeaponDataList[Random.Range(0, DataManager.instance.AllWeaponDataList.Count)];
        randomSelecter.ImageChange("Weapon", randomWeapon.name, false);

        yield return null;

        images[number].sprite = randomSelecter.GetComponent<SpriteRenderer>().sprite;

        StartCoroutine(SetRandomImage(number));
    }

    void SelectOption(int number)
    {
        if (itemPrice[number] > TurnManager.instance.player.gold) return;

        TurnManager.instance.player.ChangeGold(-itemPrice[number]);

        StageManager.instance.rewardManager.SetTraderReward(item[number]);

        SetSoldOutOption(number);
    }

    void SetSoldOutOption(int number)
    {
        nameTexts[number].text = "품절";
        images[number].color = new Color(1, 1, 1, 0.5f);
        buttons[number].interactable = false;
        priceTexts[number].text = "-";
}

    void FinishStoreRound()
    {
        leaveButton.interactable = false;
        Deck.instance.GetComponent<RectTransform>().DOAnchorPosY(-1000, 0.8f).OnComplete(() => {
            Deck.instance.ChangeParent(UIManager.instance.defaultScreen.transform);
            Deck.instance.GetComponent<RectTransform>().DOAnchorPosY(-480, 0.2f).OnComplete(() => { traderScreen.gameObject.SetActive(false); });
            Fade(false, null, () => StageManager.instance.FinishTraderRound());
        });
    }

}
