using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cinemachine;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public Transform defaultScreen, traderScreen;
    public RewardScreen rewardScreen;


    [Header("Canvas")]
    public Canvas[] canvases;

    [Header("카메라")]
    public CinemachineVirtualCamera vcam;

    [Header("오버레이")]
    public ConfirmPopup confirmPopup;
    public RectTransform centerFloat;

    [Header("재화")]
    public TextMeshProUGUI goldText;

    [Header("전투 관련")]
    public Image[] heartImages;
    public Slider enemyHealthSlider;
    public RectTransform playerHeartRect;
    public TextMeshProUGUI playerArmorText, enemyArmorText, enemyHealthText;
    public TextMeshProUGUI healthText, magicText;
    public GameObject playerUI, enemyUI;

    [Header("전투 관련 - Player")]
    public GameObject armorObejct;
    public Sprite heartIcon, armorIcon;

    [Header("전투 관련 - Float")]
    public FloatController floatController;

    [Header("전투 관련 - Buff")]
    public GameObject buffPrefab;
    public Transform playerBuffParent, enemyBuffParent;
    public List<Buff> playerBuffPool, enemyBuffPool;

    [Header("스테이지 관련")]
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI roundText;

    [Header("금화 보상")]
    public Transform coinParents;
    public GameObject coinPrefab;
    public List<GameObject> coinPool;

    [Header("신비")]
    public GameObject mysteryPrefab;
    public Transform mysteryParents;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if(TurnManager.instance.turn[0] > 0)
        {
            UpdateHpBar(enemyHealthSlider, TurnManager.instance.enemy.health);
        }
    }
    void UpdateHpBar(Slider hpBar, int hp)
    {
        if (hpBar.value > hp) hpBar.value = hpBar.value > hp ? hpBar.value - ((hpBar.value - hp) * 0.1f) : hp;
        else if (hpBar.value < hp) hpBar.value = hpBar.value < hp ? hpBar.value + ((hpBar.value + hp) * 0.005f) : hp;
    }

    #region Stage

    public void UpdateStage()
    {
        stageText.DOFade(0, 0.5f).OnComplete(()=> {
            stageText.text = DataManager.instance.StageRewardTable[StageManager.instance.curStage].name;
            stageText.DOFade(1, 0.5f).SetDelay(0.5f);
        });
    }

    public void UpdateRound()
    {
        var color = roundText.color;
        color.a = 1;
        roundText.DOFade(0, 0.5f).OnComplete(() => {
            roundText.color = color;
            roundText.text = "";
            roundText.DOText($"~{StageManager.instance.roundData.name}~", 1f).SetDelay(1f);
        });
    }

    public void OffStageAndRound()
    {
        stageText.DOFade(0, 0.5f);
        roundText.DOFade(0, 0.5f);
    }
    #endregion

    #region Float

    public void CenterFloat(int number, string icon, Color color)
    {
        centerFloat.gameObject.SetActive(true);
        centerFloat.anchoredPosition = new Vector2(0, 0);

        var floatText = centerFloat.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        var floatIcon = floatText.transform.GetChild(0);

        floatIcon.GetComponentInChildren<SlotImageAdapter>().ImageChange("Icon", icon, false);

        floatText.text = "+" + number;

        floatIcon.GetComponentInChildren<Image>().color = Color.white;
        floatText.color = color;;

        floatIcon.GetComponentInChildren<Image>().DOFade(0, 1);
        floatText.DOFade(0, 1);

        centerFloat.DOAnchorPosY(300, 1).OnComplete(() =>
        {
            centerFloat.gameObject.SetActive(false);
        });
    }

    #endregion

    #region Gold

    public void UpdateGold(int gold)
    {
        goldText.DOCounter(int.Parse(goldText.text), gold, 0.5f);
    }

    #endregion

    #region Player Heart & Armor

    public void SetPlayerUI(bool set)
    {
        var canvasGroup = playerUI.GetComponent<CanvasGroup>();
        if (set)
        {
            playerUI.SetActive(set);
            canvasGroup.alpha = 0;
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, 0.3f).SetEase(Ease.InCubic);
        }
        else
        {

            canvasGroup.alpha = 0;
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
            playerUI.SetActive(set));
        }
    }

    public void UpdatePlayerUI(int curHeart, int curArmor)
    {
        UpdatePlayerHeart(curHeart);
        UpdatePlayerArmor(curArmor);
    }

    void UpdatePlayerHeart(int curHeart)
    {
        foreach (Image i in heartImages) i.gameObject.SetActive(false);

        for (int i = 0; i < TurnManager.instance.player.maxHeart; i++)
        {
            heartImages[i].gameObject.SetActive(true);
            heartImages[i].color = i < curHeart ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
        }

        playerHeartRect.sizeDelta = new Vector2(TurnManager.instance.player.maxHeart * 100 + 100, 100);

        if(playerHeartRect.sizeDelta.x > 700)
        {
            float ratio = 700 / playerHeartRect.sizeDelta.x;
            playerHeartRect.localScale = new Vector3(ratio, ratio, 1);
        }
    }

    void UpdatePlayerArmor(int curArmor)
    {
        armorObejct.gameObject.SetActive(false);

        foreach (Image i in heartImages) i.transform.GetChild(0).gameObject.SetActive(false);

        if ( curArmor <= TurnManager.instance.player.maxHeart )
        {
            for (int i = 0; i < curArmor; i++) heartImages[i].transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            foreach (Image i in heartImages) i.transform.GetChild(0).gameObject.SetActive(true);

            armorObejct.gameObject.SetActive(true);
            playerArmorText.text = ( curArmor - TurnManager.instance.player.maxHeart).ToString();
        }

    }

    public void FloatText(FloatData floatData)
    {
        floatController.Float(floatData);
    }

    public void FloatDamage(FloatData floatData)
    {
        floatController.FloatDamage(floatData);
    }

    public void UpdateCostUI()
    {
        UpdatePlayerHealth();
        UpdatePlayerMagic();
    }

    void UpdatePlayerHealth()
    {
        healthText.text = $"{Hand.instance.healthCost}/{TurnManager.instance.player.health}";
    }

    void UpdatePlayerMagic()
    {
        magicText.text = $"{Hand.instance.magicCost}/{TurnManager.instance.player.magic}";
    }

    #endregion

    #region Enemy Health & Armor
    public void SetEnemyUI(bool set, System.Action action)
    {
        var canvasGroup = enemyUI.GetComponent<CanvasGroup>();
        if (set)
        {
            enemyUI.SetActive(set);
            canvasGroup.alpha = 0;
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if(action != null) action();
            });
        }
        else
        {
            
            canvasGroup.alpha = 1;
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, 0.3f).SetEase(Ease.InCubic).OnComplete(()=>
            {
                enemyUI.SetActive(set);
                if (action != null) action();
            });
        }
    }

    public void UpdateEnemyUI(int curHeart, int curArmor, int maxHealth = -1)
    {
        UpdateEnemyHealth(curHeart, maxHealth);
        UpdateEnemyArmor(curArmor);
    }

    void UpdateEnemyHealth(int curHealth, int maxHealth)
    {
        if(maxHealth != -1)
        {
            enemyHealthSlider.maxValue = maxHealth;
        }

        //enemyHealthSlider.value = curHealth;
        enemyHealthText.text = curHealth.ToString();
    }

    void UpdateEnemyArmor(int curArmor)
    {
        enemyArmorText.text = curArmor.ToString();
    }

    #endregion

    #region Buff

    public void UpdatePlayerBuff(List<string> buffList)
    {
        var buffs = playerBuffPool;

        foreach (string i in buffList)
        {
            var buffData = i.Split(':'); // { 버프이름, 수량 }
            Buff buff = buffs.Find(x => x.buff == buffData[0]);

            if (buff == null )
            {
                buff = Instantiate(buffPrefab, playerBuffParent).GetComponent<Buff>();
                buff.Set(buffData[0], buffData[1]);
                playerBuffPool.Add(buff);
            }
            else
            {
                buff.gameObject.SetActive(true);
                buff.Set(buffData[0], buffData[1]);
                buff.UpdateCount(buffData[1]);
            }
        }
        foreach(Buff i in buffs.Where(x => !buffList.Select(x => x.Split(':')[0]).Contains(x.buff)))
        {
            i.gameObject.SetActive(false);
        }
    }
    public void UpdateEnemyBuff(List<string> buffList)
    {
        var buffs = enemyBuffPool;

        foreach (string i in buffList)
        {
            var buffData = i.Split(':'); // { 버프이름, 수량 }
            Buff buff = buffs.Find(x => x.buff == buffData[0]);

            if (buff == null)
            {
                buff = Instantiate(buffPrefab, enemyBuffParent).GetComponent<Buff>();
                buff.Set(buffData[0], buffData[1]);
                enemyBuffPool.Add(buff);
            }
            else
            {
                buff.gameObject.SetActive(true);
                buff.UpdateCount(buffData[1]);
            }
        }
        foreach (Buff i in buffs.Where(x => !buffList.Select(x => x.Split(':')[0]).Contains(x.buff)))
        {
            i.gameObject.SetActive(false);
        }
    }

    #endregion

    #region

    public void AddMystery(string name)
    {
        Instantiate(mysteryPrefab, mysteryParents).GetComponent<Mystery>().Set(name);
    }

    #endregion

    #region Camera
    public void ShakeCamera(float _power = 1, float _time = 0.3f)
    {
        CinemachineBasicMultiChannelPerlin tmpVcamShaker =
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        tmpVcamShaker.m_AmplitudeGain = _power;

        DOTween.To(() =>
        tmpVcamShaker.m_AmplitudeGain, x =>
        tmpVcamShaker.m_AmplitudeGain = x, 0f, _time);
    }

    public void ShakeCameraByDamage(float damage)
    {
        if (damage < 10) return;

        ShakeCamera(damage / 10, 0.5f);
    }

    #endregion

    #region Guitar

    [ContextMenu("Coin Spread")]
    public void CoinSpread(int count)
    {
        for(int i = 0; i<count; i++)
        {
            var coin = coinPool.FirstOrDefault(x => !x.gameObject.activeSelf);

            if (coin == null)
            {
                coin = Instantiate(coinPrefab, coinParents);
                coinPool.Add(coin);
            }

            coin.gameObject.SetActive(true);

            StartCoroutine(CoCoinSpread(coin, i));
        }
    }

    IEnumerator CoCoinSpread(GameObject coin, int number)
    {
        var coinImage = coin.GetComponent<Image>();
        var coinRect = coin.GetComponent<RectTransform>();

        var time = Random.Range(0.5f, 1f);

        coinImage.color = Color.white;
        coinRect.anchoredPosition = Vector2.zero;

        yield return new WaitForSeconds(0.1f * number);

        coinImage.DOFade(0, time).SetEase(Ease.InQuad);
        coinRect.DOAnchorPosX(Random.Range(-200, 200), time);
        coinRect.DOAnchorPosY(Random.Range(200,400), time).SetEase(Ease.OutCubic).OnComplete(()=> coin.SetActive(false));
    }

    #endregion

}
