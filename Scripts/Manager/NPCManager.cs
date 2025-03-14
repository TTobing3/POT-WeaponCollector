using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class NPCManager : MonoBehaviour
{
    NpcData npcData;

    [Header("UI")]
    public GameObject screen;
    public CanvasGroup npcCanvasGroup;

    public TextMeshProUGUI titleText;
    public SlotImageAdapter npcImage;
    public TextMeshProUGUI[] nameText, desText;
    public SlotImageAdapter[] image;
    public Button[] buttons;

    void Start()
    {
        buttons[0].interactable = false;
        buttons[1].interactable = false;

        buttons[0].onClick.AddListener(() => { ActEffect(npcData.options[0]); });
        buttons[1].onClick.AddListener(() => { ActEffect(npcData.options[1]); });
    }

    public void Fade(bool fadeIn, System.Action inCall = null, System.Action outCall = null)
    {
        if (fadeIn)
        {
            npcCanvasGroup.alpha = 0;
            DOTween.To(() => npcCanvasGroup.alpha, x => npcCanvasGroup.alpha = x, 1, 0.3f).SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    if (inCall != null) inCall();
                });
        }
        else
        {
            npcCanvasGroup.alpha = 1;
            DOTween.To(() => npcCanvasGroup.alpha, x => npcCanvasGroup.alpha = x, 0, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if (outCall != null) outCall();
            });
        }

    }

    public void NpcSet(string name)
    {
        var npcData = DataManager.instance.AllNpcDatas[name];

        this.npcData = npcData;

        Set();

        titleText.text = npcData.eventName;
        npcImage.ImageChange("Character", npcData.name, false);

        for(int i = 0; i<2; i++)
        {
            image[i].ImageChange("NPC", npcData.icons[i], false);
            nameText[i].text = npcData.options[i];
            desText[i].text = npcData.effects[i];

        }
    }

    void Set(bool set = true)
    {
        if(set)
        {
            screen.SetActive(true);
            Fade(true, () =>
            {
                buttons[0].interactable = true;
                buttons[1].interactable = true;
            });
        }
        else
        {
            buttons[0].interactable = false;
            buttons[1].interactable = false;
            Fade(false, null, () =>
            {
                screen.SetActive(false);
            });
        }
    }

    void ActEffect(string effectName)
    {
        Set(false);

        switch (effectName)
        {
            case "가방 확장":
                //가방 확장
                for (int i = 0; i < 4; i++)
                    Deck.instance.AddDeckSlot();
                break;

            case "요령 전수":
                //요령 전수
                Hand.instance.AddHandSlot();
                Hand.instance.AddHandSlot();
                break;

            case "체력 단련":
                TurnManager.instance.player.AddHealth(2);
                break;

            case "마력 단련":
                TurnManager.instance.player.AddMagic(2);
                break;

            case "넘치는 금화":
                // 두 개의 이벤트 딜레이 주고 실행
                StartCoroutine(ActTwoActionDelay(
                    () =>
                    {
                        Fade(false, null, () =>
                        {
                            TurnManager.instance.player.DecreaseHeart();
                            TurnManager.instance.player.HitAnimation();
                            TurnManager.instance.player.HitWhiteFlash();
                            UIManager.instance.ShakeCameraByDamage(100);
                        });
                    },
                    () =>
                    {
                        Fade(true, () =>
                        {
                            var rewardCoin = Random.Range(5, 10);

                            TurnManager.instance.player.ChangeGold(rewardCoin);
                            UIManager.instance.CenterFloat(rewardCoin, "금화", new Color(1, 0.7f, 0, 1));
                            UIManager.instance.CoinSpread(rewardCoin);

                            FinishTalk(effectName);
                        }, null);
                    }
                    ,
                    0.5f
                    ));

                return;

            case "쓸데없는 무기":

                // 두 개의 이벤트 딜레이 주고 실행, 공격 후 진행이라 npc 화면 끄고 공격 후 다시 키고 보상
                StartCoroutine(ActTwoActionDelay(
                    () => 
                    {
                        Fade(false, null, () =>
                        {
                            TurnManager.instance.player.DecreaseHeart();
                            TurnManager.instance.player.HitAnimation();
                            TurnManager.instance.player.HitWhiteFlash();
                            UIManager.instance.ShakeCameraByDamage(100);
                        });
                    },
                    () =>
                    {
                        Fade(true, () => 
                        {
                            var weapon = DataManager.instance.AllWeaponDataByRank["고물"][Random.Range(0, DataManager.instance.AllWeaponDataByRank["고물"].Count)];

                            StageManager.instance.rewardManager.SetWeaponReward(weapon, () => { FinishTalk(effectName); });
                        }, null);
                    }
                    ,
                    0.5f
                    ));

                return;

            case "보물상자 열기":

                var weapon = DataManager.instance.AllWeaponDataList[ Random.Range( 0, DataManager.instance.AllWeaponDataList.Count ) ];

                StageManager.instance.rewardManager.SetWeaponReward(weapon.name, () => { FinishTalk(effectName); });
                return;

            case "무시하고 지나가기":
                break;

            default:
                effectName = "무시하고 지나가기";
                break;
        }

        FinishTalk(effectName);
    }

    IEnumerator ActTwoActionDelay(System.Action firstAction, System.Action secondAction, float delay )
    {
        firstAction();
        yield return new WaitForSeconds(delay);
        secondAction();
    }

    void FinishTalk(string effectName)
    {
        TalkManager.instance.StartTalks(
            DataManager.instance.AllTalkDatas[effectName].talkList,
            new List<System.Action>() { Finish }
            );
    }
    
    void Finish()
    {
        Set(false);

        StageManager.instance.FinishNpcRound();
    }
}
