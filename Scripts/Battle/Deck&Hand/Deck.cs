using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Deck : MonoBehaviour
{
    public static Deck instance;
    public GameObject deckSlot;
    public Transform content;

    public List<DeckSlot> deckList;

    public int deckWeaponCount
    {
        get
        {
            return deckList.Count(deck => deck.slot.weaponData.number != -1);
        }
    }
    public List<DeckSlot> deckWeaponList
    {
        get
        {
            return deckList.Where(deck => deck.slot.weaponData.number != -1).ToList();
        }
    }

    public bool changing = false;

    RectTransform rect;
    #region System
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        rect.DOAnchorPosY(-480, 0.5f);
    }
    #endregion
    #region UI

    public void MoveDeck(bool isOpen)
    {
        if (changing) return;

        if(isOpen)
        {
            rect.DOAnchorPosY(-240,0.5f);
        }
        else
        {
            rect.DOAnchorPosY(-480, 0.5f);
        }
    }

    public void MoveOutDeck()
    {
        DOTween.Kill(rect);

        rect.DOAnchorPosY(-960, 0.5f);
        rect.DOAnchorPosX(-500, 2f);
    }

    public void MoveInDeck()
    {
        DOTween.Kill(rect);

        rect.DOAnchorPosY(-480, 0.5f);
    }

    public void ChangeParent(Transform parent)
    {
        transform.SetParent(parent);

        var order = parent.root.GetComponent<Canvas>().sortingOrder;

        foreach (DeckSlot i in deckList) i.ChangeParticleOrder(order+1);
    }

    #endregion
    #region Info

    public string GetMostGroup()
    {
        return deckList
            .Where(deck => deck.GetWeaponData.number != -1)
            .SelectMany(deck => deck.GetWeaponData.group)
            .GroupBy(group => group)
            .OrderByDescending(group => group.Count())
            .FirstOrDefault()?.Key;
    }

    public string GetMostType()
    {
        return deckList
            .Where(deck => deck.GetWeaponData.number != -1)
            .Select(deck => deck.GetWeaponData.type) // �� ���� ����Ʈ�� ��źȭ
            .GroupBy(type => type)
            .OrderByDescending(type => type.Count())
            .FirstOrDefault()?.Key;
    }

    // 0 : 1���� / 1 : 2����
    public string GetMostAlignment(int number)
    {
        string alignment = "�߸�";

        var alignments = deckList
            .Where(deck => deck.GetWeaponData.number != -1)
            .Select(deck => deck.GetWeaponData.alignment[number]) // alignment�� ù ��° ��� ����
            .GroupBy(alignment => alignment)                 // alignment�� �������� �׷�ȭ
            .Select(group => new { Alignment = group.Key, Count = group.Count() })
            .OrderByDescending(alignment => alignment.Count)
            .ToList();


        for(int i = 0; i<3 - alignments.Count; i++)
        {  
            alignments.Add(new { Alignment = "��Ÿ", Count = 0 });
        }

        if (alignments[0].Count != alignments[1].Count)
        {
            alignment = alignments[0].Alignment;
        }
        else if ( alignments[1].Count != alignments[2].Count)
        {
            if(alignments[0].Alignment == "�߸�")
            {
                alignment = alignments[1].Alignment;
            }
            else if (alignments[1].Alignment == "�߸�")
            {
                alignment = alignments[0].Alignment;
            }
        }

        return alignment;
    }

    #endregion
    #region Game

    public void GainWeapon(string weapon)
    {
        foreach(DeckSlot i in deckList)
        {
            if(i.slot.weaponData.number == -1)
            {
                i.SetSlot(weapon);
                return;
            }
        }
    }

    public DeckSlot AddDeckSlot()
    {
        var slot = Instantiate(deckSlot, content).GetComponent<DeckSlot>();

        deckList.Add(slot);

        return slot;
    }
    public void RemoveDeckSlot()
    {
        if (deckList.Count < 2) return;

        Destroy(deckList[deckList.Count - 1].gameObject);

        deckList.RemoveAt(deckList.Count - 1);
    }
    //���⼭�� �׷� �½��� �̷��͸� ���ָ� ��, �׳� ������°� �߰��ϰų� ���������� ������ �� ����

    public void RemoveDeckSlot(DeckSlot deckSlot)
    {
        var index = deckList.IndexOf(deckSlot);
        Destroy(deckList[index].gameObject);

        deckList.RemoveAt(index);
    }

    #endregion

    #region TempUpgrade

    public void ResetTempUpgrade(bool isTurn)
    {
        foreach (DeckSlot i in deckList)
        {
            i.ResetTempUpgrade(isTurn);
        }
    }

    #endregion

}
