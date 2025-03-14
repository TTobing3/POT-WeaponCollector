using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum StageType { Etc, Battle, Dwarf, Elf, Trader, Boss, Core };

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    public GameOverManager gameOverManager;

    [HideInInspector]
    public MysteryManager mysteryManager;
    [HideInInspector]
    public RewardManager rewardManager;
    [HideInInspector]
    public StoreManager storeManager;
    [HideInInspector]
    public NPCManager npcManager;


    [Header("스테이지 정보")]

    public RoundData roundData;

    public int curStage;
    public int curRound = 1; // 0 접근자는 StageMap에서 스테이지 이름 접근

    [Header("신비")]
    public List<MysteryData> mysteries;

    [Header("NPC")]
    public Transform player;
    public Transform npc;
    public Transform enemy;

    [Header("Animator")]
    public Animator playerAnimator;

    [Header("Deck&Hand")]
    public Deck deck;
    public Hand hand;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        mysteryManager = GetComponent<MysteryManager>();
        rewardManager = GetComponent<RewardManager>();
        storeManager = GetComponent<StoreManager>();
        npcManager = GetComponent<NPCManager>();
    }

    void Start()
    {
        GameStart();
    }

    void GameStart()
    {
        print("----- 게임 시작 세팅 -----");
        //deck.AddDeckSlot();
        //hand.AddHandSlot();

        print("무기 획득");
        deck.GainWeapon("용병의 마체테");

        UIManager.instance.UpdateStage();
        UIManager.instance.SetPlayerUI(true);

        print("----- 게임 시작 세팅 끝 -----");

        StartRound();
    }

    public void GameOver()
    {
        gameOverManager.Set();
    }

    public void StartRound()
    {
        SetRound(DataManager.instance.StageMap[curStage][curRound].Split('/'));
        UIManager.instance.UpdateRound();
    }

    public void SetRound(string[] type)
    {
        SelectRandomRound(type);

        switch (type[0])
        {
            case "전투":
                SetBattleRound();
                break;

            case "NPC":
                SetNpcRound(roundData.target);
                break;

            case "상인":
                SetTraderRound();
                break;

            case "보스":
                SetBossRound();
                break;

            case "시작":
                SetStartRound();
                break;

            default:
                break;
        }
    }

    void SelectRandomRound(string[] type)
    {
        // 라운드 데이터 저장
        if (type[0] == "NPC") // NPC 라운드
        {
            if (type[1] == "무작위")
            {
                var curTierRoundList = DataManager.instance.AllRoundDataList
                    .Where(roundData => roundData.tier == curStage)
                    .ToList();

                var curTypeRoundList = curTierRoundList
                    .Where(roundData => roundData.type == type[0])
                    .Where(roundData => roundData.target != "드워프" && roundData.target != "엘프") // 특수 라운드 제외
                    .ToList();

                var curRound = curTypeRoundList[Random.Range(0, curTypeRoundList.Count)];

                roundData = curRound;
            }
            else // 특수 NPC, 드워프 혹은 엘프 라운드
            {

                var curTypeRoundList = DataManager.instance.AllRoundDataList
                    .Where(roundData => roundData.type == type[0])
                    .Where(roundData => roundData.target == type[1]) // 특수 라운드 전용, 타겟으로 판별
                    .ToList();

                var curRound = curTypeRoundList[Random.Range(0, curTypeRoundList.Count)];

                roundData = curRound;
            }
        }
        else // 일반 라운드
        {
            var curTierRoundList = DataManager.instance.AllRoundDataList
                .Where(roundData => roundData.tier == curStage)
                .ToList();

            var curTypeRoundList = curTierRoundList
                .Where(roundData => roundData.type == type[0])
                .ToList();

            var curRound = curTypeRoundList[Random.Range(0, curTypeRoundList.Count)];

            roundData = curRound;
        }


        print($"라운드 세팅 : 현재 스테이지 {roundData} / 현재 라운드 {roundData.name} / 라운드 타입 {roundData.type}");

    }

    #region 시작 라운드
    void SetStartRound()
    {
        SetCharacter(FinishStartRound);
    }

    void FinishStartRound()
    {
        OutCharacter();
        FinishRound();
    }
    #endregion

    #region 전투
    void SetBattleRound()
    {
        SetCharacter(TurnManager.instance.StartBattle);
    }
    public void FinishBattleRound()
    {
        OutCharacter();
        rewardManager.SetBattleRewardScreen();
    }
    #endregion

    #region 보스

    void SetBossRound()
    {
        SetCharacter(TurnManager.instance.StartBattle);
    }

    public void FinishBossRound()
    {
        OutCharacter();

        FinishStage();
    }

    #endregion

    #region NPC
    void SetNpcRound(string target)
    {
        SetCharacter(()=>npcManager.NpcSet(target));
    }

    public void FinishNpcRound()
    {
        OutCharacter();
        FinishRound();
    }
    #endregion

    #region 상인

    void SetTraderRound()
    {
        SetCharacter(storeManager.Set);
    }

    public void FinishTraderRound()
    {
        OutCharacter();
        FinishRound();
    }

    #endregion

    void SetCharacter(System.Action action = null)
    {
        if(roundData.type == "전투" || roundData.type == "보스")
        {
            enemy.gameObject.SetActive(true);

            enemy.GetComponent<Enemy>().Set(DataManager.instance.AllEnemyDatas[roundData.target]);
            enemy.GetComponent<SpriteRenderer>().color = Color.white;

            enemy.DOMoveX(5, 1).OnComplete(() => { SetTalk(action); });

            enemy.GetComponent<SpriteResolver>().SetCategoryAndLabel("Character", roundData.target);
        }
        else
        {
            npc.gameObject.SetActive(true);
            npc.GetComponent<SpriteRenderer>().color = Color.white;

            npc.DOMoveX(5, 1).OnComplete(()=> { SetTalk(action); });

            npc.GetComponent<SpriteResolver>().SetCategoryAndLabel("Character", roundData.target);
        }
    }

    void OutCharacter()
    {
        if (roundData.type == "전투" || roundData.type == "보스")
        {
            enemy.GetComponent<SpriteRenderer>().DOFade(0, 0.5f).OnComplete(() =>
            {
                enemy.transform.position = new Vector2(10, 0);
                enemy.GetComponent<SpriteRenderer>().color = Color.white;
                enemy.gameObject.SetActive(false);
            });
        }
        else
        {
            npc.GetComponent<SpriteRenderer>().DOFade(0, 0.5f).OnComplete(() =>
            {
                npc.transform.position = new Vector2(10, 0);
                npc.GetComponent<SpriteRenderer>().color = Color.white;
                npc.gameObject.SetActive(false);
            });
        }
    }

    void SetTalk(System.Action action = null)
    {
        if (action == null) action = () => { };

        npc.GetComponent<SpriteResolver>().SetCategoryAndLabel("Character", roundData.target);

        TalkManager.instance.StartTalks(
            DataManager.instance.AllTalkDatas[roundData.name].talkList,
            new List<System.Action>() { action });
    }

    public void FinishRound()
    {
        Tooltip.instance.OffTooltip();
        StartCoroutine(CoFinishRound(1));
    }

    public IEnumerator CoFinishRound(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (curRound == DataManager.instance.StageMap[curStage].Length - 1) 
        {
            NextStage();
        }
        else
        {
            NextRound();
        }
    }

    public void NextRound()
    {
        curRound++;

        RoundWalk();
    }

    public void FinishStage()
    {
        mysteryManager.Set();
        UIManager.instance.OffStageAndRound();
    }

    public void NextStage()
    {
        curRound = 1;
        curStage++;
        UIManager.instance.UpdateStage();

        RoundWalk();
    }

    void RoundWalk()
    {
        StartCoroutine(CoRoundWalk());

        playerAnimator.SetInteger("state", 1);
        playerAnimator.SetTrigger("act");

        Delay(() =>
        {
            playerAnimator.SetInteger("state", 0);
            playerAnimator.SetTrigger("act");
        }, 1);

        //player.DOMoveY(player.position.y + 1, 0.2f).SetDelay(0f).OnComplete(() => { player.DOMoveY(player.position.y - 1, 0.2f); });
        //player.DOMoveY(player.position.y + 1, 0.2f).SetDelay(0.4f).OnComplete(() => { player.DOMoveY(player.position.y - 1, 0.2f); });
        //player.DOMoveY(player.position.y + 1, 0.2f).SetDelay(0.8f).OnComplete(() => { player.DOMoveY(player.position.y - 1, 0.2f); });

    }

    IEnumerator CoRoundWalk()
    {
        yield return new WaitForSeconds(0.4f);

        StartRound();
    }

    //

    void OnDestroy()
    {
        DOTween.KillAll();
    }

    void Delay(System.Action action, float delay)
    {
        StartCoroutine(CoDelay(action, delay));
    }

    IEnumerator CoDelay(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);

        action();
    }
}
