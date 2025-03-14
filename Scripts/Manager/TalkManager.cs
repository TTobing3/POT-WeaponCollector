using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class TalkManager : MonoBehaviour
{
    public static TalkManager instance;

    public System.Action NextAction; 


    [Header("Talk")]
    public GameObject talkPanel;
    public Image portrait;
    public TextMeshProUGUI talkName;
    public TextMeshProUGUI talkText;
    public List<string[]> nextTalks = new List<string[]>();

    public Button nextButton;

    //일러스트 존재 확인
    public SpriteResolver portraitResolver;
    public SpriteLibrary spriteLibrary;

    public CanvasGroup talkCanvasGroup;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        talkCanvasGroup = GetComponent<CanvasGroup>();
        nextButton.onClick.AddListener(TalkNext);
    }

    public void Fade(bool fadeIn, System.Action inCall = null, System.Action outCall = null)
    {
        if (fadeIn)
        {
            talkCanvasGroup.alpha = 0;
            DOTween.To(() => talkCanvasGroup.alpha, x => talkCanvasGroup.alpha = x, 1, 0.3f).SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    if (inCall != null) inCall();
                });
        }
        else
        {
            talkCanvasGroup.alpha = 1;
            DOTween.To(() => talkCanvasGroup.alpha, x => talkCanvasGroup.alpha = x, 0, 0.3f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if (outCall != null) outCall();
            });
        }

    }

    public void StartTalks(List<string[]> talkList, List<System.Action> _action = null)
    {
        talkPanel.SetActive(true);

        nextButton.interactable = false;
        Fade(true, ()=> { nextButton.interactable = true; });

        NextAction = null;
        nextTalks = new List<string[]>();

        if (_action != null && _action.Count != 0)
        {
            for (int i = 0; i < _action.Count; i++) NextAction += _action[i];
        }

        foreach (string[] i in talkList) nextTalks.Add(i);

        TalkNext();
    }


    public void TalkNext()
    {
        if (nextTalks.Count <= 0)
        {
            FinishTalk();
        }
        else
        {
            SetTalk(nextTalks[0][0]);
            Talk(nextTalks[0][1]);
            nextTalks.RemoveAt(0);
        }
    }


    void Talk(string _talk)
    {
        talkText.DOKill();
        talkText.text = "";
        talkText.DOText(_talk, 0.1f);
    }

    bool CheckPortrait(string _name)
    {
        return spriteLibrary.spriteLibraryAsset.GetCategoryLabelNames("Character").ToList().Contains(_name);
    }

    void SetTalk(string _name)
    {
        talkName.text = _name;

        if (CheckPortrait(_name))
        {
            portrait.GetComponent<SpriteResolver>().SetCategoryAndLabel("Character", _name);
            portrait.sprite = portrait.GetComponent<SpriteRenderer>().sprite;
            portrait.gameObject.SetActive(true);
        }
        else
        {
            portrait.gameObject.SetActive(false);
        }
    }


    void FinishTalk()
    {
        nextButton.interactable = false;
        Fade(false, null, ()=>
        {
            talkPanel.gameObject.SetActive(false);
            if (NextAction != null) NextAction();
        });
    }

}