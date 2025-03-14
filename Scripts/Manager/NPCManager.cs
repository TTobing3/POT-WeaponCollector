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
            case "���� Ȯ��":
                //���� Ȯ��
                for (int i = 0; i < 4; i++)
                    Deck.instance.AddDeckSlot();
                break;

            case "��� ����":
                //��� ����
                Hand.instance.AddHandSlot();
                Hand.instance.AddHandSlot();
                break;

            case "ü�� �ܷ�":
                TurnManager.instance.player.AddHealth(2);
                break;

            case "���� �ܷ�":
                TurnManager.instance.player.AddMagic(2);
                break;

            case "��ġ�� ��ȭ":
                // �� ���� �̺�Ʈ ������ �ְ� ����
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
                            UIManager.instance.CenterFloat(rewardCoin, "��ȭ", new Color(1, 0.7f, 0, 1));
                            UIManager.instance.CoinSpread(rewardCoin);

                            FinishTalk(effectName);
                        }, null);
                    }
                    ,
                    0.5f
                    ));

                return;

            case "�������� ����":

                // �� ���� �̺�Ʈ ������ �ְ� ����, ���� �� �����̶� npc ȭ�� ���� ���� �� �ٽ� Ű�� ����
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
                            var weapon = DataManager.instance.AllWeaponDataByRank["��"][Random.Range(0, DataManager.instance.AllWeaponDataByRank["��"].Count)];

                            StageManager.instance.rewardManager.SetWeaponReward(weapon, () => { FinishTalk(effectName); });
                        }, null);
                    }
                    ,
                    0.5f
                    ));

                return;

            case "�������� ����":

                var weapon = DataManager.instance.AllWeaponDataList[ Random.Range( 0, DataManager.instance.AllWeaponDataList.Count ) ];

                StageManager.instance.rewardManager.SetWeaponReward(weapon.name, () => { FinishTalk(effectName); });
                return;

            case "�����ϰ� ��������":
                break;

            default:
                effectName = "�����ϰ� ��������";
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
