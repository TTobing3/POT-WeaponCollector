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


    [Header("�������� ����")]

    public RoundData roundData;

    public int curStage;
    public int curRound = 1; // 0 �����ڴ� StageMap���� �������� �̸� ����

    [Header("�ź�")]
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
        print("----- ���� ���� ���� -----");
        //deck.AddDeckSlot();
        //hand.AddHandSlot();

        print("���� ȹ��");
        deck.GainWeapon("�뺴�� ��ü��");

        UIManager.instance.UpdateStage();
        UIManager.instance.SetPlayerUI(true);

        print("----- ���� ���� ���� �� -----");

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
            case "����":
                SetBattleRound();
                break;

            case "NPC":
                SetNpcRound(roundData.target);
                break;

            case "����":
                SetTraderRound();
                break;

            case "����":
                SetBossRound();
                break;

            case "����":
                SetStartRound();
                break;

            default:
                break;
        }
    }

    void SelectRandomRound(string[] type)
    {
        // ���� ������ ����
        if (type[0] == "NPC") // NPC ����
        {
            if (type[1] == "������")
            {
                var curTierRoundList = DataManager.instance.AllRoundDataList
                    .Where(roundData => roundData.tier == curStage)
                    .ToList();

                var curTypeRoundList = curTierRoundList
                    .Where(roundData => roundData.type == type[0])
                    .Where(roundData => roundData.target != "�����" && roundData.target != "����") // Ư�� ���� ����
                    .ToList();

                var curRound = curTypeRoundList[Random.Range(0, curTypeRoundList.Count)];

                roundData = curRound;
            }
            else // Ư�� NPC, ����� Ȥ�� ���� ����
            {

                var curTypeRoundList = DataManager.instance.AllRoundDataList
                    .Where(roundData => roundData.type == type[0])
                    .Where(roundData => roundData.target == type[1]) // Ư�� ���� ����, Ÿ������ �Ǻ�
                    .ToList();

                var curRound = curTypeRoundList[Random.Range(0, curTypeRoundList.Count)];

                roundData = curRound;
            }
        }
        else // �Ϲ� ����
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


        print($"���� ���� : ���� �������� {roundData} / ���� ���� {roundData.name} / ���� Ÿ�� {roundData.type}");

    }

    #region ���� ����
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

    #region ����
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

    #region ����

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

    #region ����

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
        if(roundData.type == "����" || roundData.type == "����")
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
        if (roundData.type == "����" || roundData.type == "����")
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
