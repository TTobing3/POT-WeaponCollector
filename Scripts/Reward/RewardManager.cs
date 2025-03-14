using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class RewardManager : MonoBehaviour
{
    public System.Action WeaponRewardFinishAction;

    public RewardScreen rewardScreen;

    [Header("���� ����")]
    public GameObject pocketRewardScreen;
    public GameObject trainRewardScreen;
    public GameObject weaponRewardScreen;
    public GameObject weaponChangeScreen;

    [Header("��ư")]
    public Button pocketButton;
    public Button takeRestButton;
    public Button[] trainButtons;
    public Button[] weaponButtons; // gain / cancle
    public Button[] changeButtons; // cancle

    [Header("-")]
    public string selectedWeapon;

    void Awake()
    {
        pocketButton.onClick.AddListener(OpenPocketScreen);

        takeRestButton.onClick.AddListener(TakeRest);

        trainButtons[0].onClick.AddListener(() => { SelectTrain(0); });
        trainButtons[1].onClick.AddListener(() => { SelectTrain(1); });
        trainButtons[2].onClick.AddListener(() => { SelectTrain(2); });

        weaponButtons[0].onClick.AddListener(GainWeapon);

        weaponButtons[1].onClick.AddListener(CancleWeaponGain);

        changeButtons[0].onClick.AddListener(FinishChangeWeaponScreen);
    }

    public void SetBattleRewardScreen()
    {
        WeaponRewardFinishAction += StageManager.instance.FinishRound;

        selectedWeapon = DataManager.instance.GetWeaponReward();

        SetPocket();
    }

    public void SetTraderReward(string weapon)
    {
        WeaponRewardFinishAction = null;

        selectedWeapon = weapon;

        SetWeaponRewardScreen();
    }

    public void SetWeaponReward(string weapon, System.Action action)
    {
        WeaponRewardFinishAction += action;

        selectedWeapon = weapon;

        SetWeaponRewardScreen();
    }

    #region �ָӴ�

    void SetPocket()
    {
        pocketRewardScreen.SetActive(true);
        rewardScreen.Fade(true);

        UIManager.instance.rewardScreen.SetPocketScreen();
    }

    void OpenPocketScreen()
    {
        var coinRewardRange = DataManager.instance.StageRewardTable[StageManager.instance.curStage].coinRewardRange;
        var rewardCoin = Random.Range(coinRewardRange[0], coinRewardRange[1]);

        TurnManager.instance.player.ChangeGold(rewardCoin);
        UIManager.instance.CenterFloat(rewardCoin, "��ȭ", new Color(1, 0.7f, 0, 1));
        UIManager.instance.CoinSpread(rewardCoin);

        var delay = 1f + (0.1f * rewardCoin);

        pocketButton.interactable = false;

        pocketButton.GetComponent<Image>().DOFade(0, delay).OnComplete(()=>
        {
            pocketButton.interactable = true;
            pocketButton.gameObject.SetActive(false); 
        });

        StartCoroutine(CoFinishPocketScreen(delay));
    }

    IEnumerator CoFinishPocketScreen(float delay)
    {
        yield return new WaitForSeconds(delay);

        FinishPocketScreen();
    }
    
    void FinishPocketScreen()
    {
        rewardScreen.Fade(false, null, ()=>
        {
            pocketRewardScreen.SetActive(false);
            SetTrainScreen();
        });
    }

    #endregion

    #region �Ʒ�

    void SetTrainScreen()
    {
        trainRewardScreen.SetActive(true);
        rewardScreen.Fade(true);

        var selectedTrains = Enumerable.Range(0, 16).OrderBy(x => Random.Range(0f, 1f)).Distinct().Take(3).ToList();
        UIManager.instance.rewardScreen.SetTrainScreen(selectedTrains);
    }

    void SelectTrain(int t)
    {
        for (int j = 0; j < 3; j++) trainButtons[j].interactable = false;

        var stageLevel = StageManager.instance.curStage;

        for (int i = 0; i < DataManager.instance.StageRewardTable[stageLevel].train; i++)
        {
            TurnManager.instance.player.AddTrain((int)rewardScreen.options[t].type);
        }
        
        FinishTrainScreen();
    }

    void TakeRest()
    {
        takeRestButton.interactable = false;
        TurnManager.instance.player.AddHeart();

        FinishTrainScreen();
    }

    void FinishTrainScreen()
    {
        rewardScreen.Fade(false, null, () =>
        {
            for(int i = 0; i<3; i++) trainButtons[i].interactable = true;
            takeRestButton.interactable = true;
            trainRewardScreen.SetActive(false);
            SetWeaponRewardScreen();
        });
    }

    #endregion

    #region ����

    void SetWeaponRewardScreen()
    {
        weaponRewardScreen.SetActive(true);
        rewardScreen.Fade(true);

        UIManager.instance.rewardScreen.SetWeaponScreen(selectedWeapon);
    }

    void GainWeapon()
    {
        weaponButtons[0].interactable = false;
        rewardScreen.Fade(false, null, () =>
        {
            weaponButtons[0].interactable = true;
            weaponRewardScreen.SetActive(false);
            if (Deck.instance.deckList.Count > Deck.instance.deckWeaponCount) // �ٷ� ȹ��
            {
                Deck.instance.GainWeapon(selectedWeapon);
                FinishWeaponRewardScreen();
            }
            else // �� ������ ��� ��ȯ
            {
                SetChangeWeaponScreen(); ;
            }
        });
    }

    #region ��ȯ

    void SetChangeWeaponScreen()
    {
        weaponChangeScreen.SetActive(true);
        UIManager.instance.rewardScreen.SetChangeScreen(selectedWeapon);
        rewardScreen.Fade(true, () =>
        {
            changeButtons[0].gameObject.SetActive(true);


            Deck.instance.changing = true;

            Deck.instance.GetComponent<RectTransform>().DOAnchorPosY(-800, 0.2f).OnComplete(() => {
                Deck.instance.ChangeParent(weaponChangeScreen.transform);
                Deck.instance.GetComponent<RectTransform>().DOAnchorPosY(100, 0.8f);
            });
        });
    }

    public void SelectChangeTargetSlot(DeckSlot deckSlot)
    {
        if (deckSlot.GetWeaponData.number == -1)
        {
            ChangeWeapon(deckSlot);
            FinishChangeWeaponScreen();
        }
        else
        {
            UIManager.instance.confirmPopup.gameObject.SetActive(true);
            UIManager.instance.confirmPopup.Set(
                "��ȯ",
                $"{deckSlot.slot.weaponData.name}��(��) ������ {selectedWeapon}��(��) ������ ���ðڽ��ϱ�?",
                () =>
                {
                    ChangeWeapon(deckSlot);
                    FinishChangeWeaponScreen();
                });
        }
    }

    void ChangeWeapon(DeckSlot deckSlot)
    {
        deckSlot.DropSlot();

        Deck.instance.GainWeapon(selectedWeapon);
    }

    public void FinishChangeWeaponScreen()
    {
        print("finish Change");

        if (selectedWeapon == "") return;

        selectedWeapon = "";

        changeButtons[0].interactable = false;
        changeButtons[0].gameObject.SetActive(false);

        if (StageManager.instance.roundData.type == "����")
        {
            Deck.instance.GetComponent<RectTransform>().DOAnchorPosY(-1000, 0.6f).OnComplete(() => {
                Deck.instance.ChangeParent(UIManager.instance.traderScreen.transform);
                Deck.instance.GetComponent<RectTransform>().DOAnchorPosY(-480, 0.2f).OnComplete(() => {
                    Deck.instance.changing = false;
                    weaponChangeScreen.SetActive(false); 
                });
            });
        }
        else
        {
            Deck.instance.GetComponent<RectTransform>().DOAnchorPosY(-1000, 0.6f).OnComplete(() => {
                Deck.instance.ChangeParent(UIManager.instance.defaultScreen.transform);
                Deck.instance.GetComponent<RectTransform>().DOAnchorPosY(-480, 0.2f).OnComplete(() => {
                    Deck.instance.changing = false;
                    weaponChangeScreen.SetActive(false);
                });

                FinishWeaponRewardScreen();
            });
        }

    }

    #endregion

    void CancleWeaponGain()
    {
        weaponButtons[1].interactable = false;
        FinishWeaponRewardScreen();
    }

    void FinishWeaponRewardScreen()
    {
        rewardScreen.Fade(false, null, () =>
        {
            changeButtons[0].interactable = true;
            weaponButtons[1].interactable = true;
            weaponRewardScreen.SetActive(false);

            if (WeaponRewardFinishAction != null)
            {
                WeaponRewardFinishAction();
                WeaponRewardFinishAction = null;
            }
        });
    }

    #endregion
}
