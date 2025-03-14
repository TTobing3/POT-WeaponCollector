using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
// 시너지 이름과 개수를 저장할 클래스
[System.Serializable]
public class GroupSynergy
{
    public string name;
    public int count;

    public GroupSynergy(string name)
    {
        this.name = name;
        this.count = 1;
    }

    public string GetEffect()
    {
        return DataManager.instance.AllGroupSynergyDatas[name].effects[count];
    }
}

public class Hand : MonoBehaviour
{
    public static Hand instance;

    public GameObject handSlotPrefab, groupSynergySlotPrefab;

    public int hands;
    public RectTransform handContent, groupSynergyContent;
    public List<HandSlot> cards; //UI

    public List<GroupSynergy> groupSynergyList; // 시너지 리스트
    public List<GameObject> groupSynergySlotList;

    public List<HandSlot> tempHandSlots = new List<HandSlot>();
    public List<DeckSlot> tempDeckSlots = new List<DeckSlot>();

    #region property

    public int healthCost
    {
        get
        {
            return cards
                .Where(card => card.deckSlot != null)
                .Sum(card => card.GetWeaponData.health);
        }
    }
    public int magicCost
    {
        get
        {
            return cards
                .Where(card => card.deckSlot != null)
                .Sum(card => card.GetWeaponData.magic);
        }
    }

    public List<GroupSynergy> mostSynergy
    {
        get
        {
            List<GroupSynergy> mostSynergy = new List<GroupSynergy>();

            foreach(GroupSynergy i in groupSynergyList)
            {
                if (i.count == groupSynergyList.Max(x => x.count))
                    mostSynergy.Add(i);
            }

            return mostSynergy;
        }
    }

    #endregion

    #region System
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        cards = GetComponentsInChildren<HandSlot>().ToList();
    }
    #endregion

    #region Synergy
    public void UpdateGroupSynergy()
    {
        // groupList 초기화
        groupSynergyList.Clear();

        // cards 리스트 순회
        foreach (HandSlot i in cards)
        {
            if (i.deckSlot == null) continue;

            foreach(string j in i.GetWeaponData.group)
            {
                GainSynergy(j);
            }
        }

        UpdateGroupSynergyUI();
    }
    public void UpdateGroupSynergyUI()
    {
        foreach (GameObject i in groupSynergySlotList) 
            i.gameObject.SetActive(false);

        foreach(GroupSynergy groupSynergy in groupSynergyList)
        {

            GameObject groupSynergyUI = null;

            foreach (GameObject i in groupSynergySlotList)
            {
                if (i.activeSelf) continue;
                groupSynergyUI = i;
                groupSynergyUI.gameObject.SetActive(true);
                break;
            }

            if (groupSynergyUI == null)
            {
                groupSynergyUI = Instantiate(groupSynergySlotPrefab, groupSynergyContent);
                groupSynergySlotList.Add(groupSynergyUI);
            }

            groupSynergyUI.GetComponent<GroupSynergyUI>().Set(groupSynergy);
        }
    }
    public void GainSynergy(string synergy)
    {
        var targetSynergy = groupSynergyList.Find(x => x.name == synergy);

        if (targetSynergy == null)
        {
            groupSynergyList.Add(new GroupSynergy(synergy));
        }
        else
        {
            targetSynergy.count++;
        }

        UpdateGroupSynergyUI();
    }
    public GroupSynergy GetSynergy(string name)
    {
        GroupSynergy synergy = null;
        synergy = groupSynergyList.Find(x => x.name == name);
        return synergy;
    }
    #endregion

    #region Slot
    public void AddHandSlot()
    {
        var card = Instantiate(handSlotPrefab, handContent).GetComponent<HandSlot>();

        cards.Add(card);

        HandResize(true);
    }
    public void RemoveHandSlot()
    {
        if (cards.Count < 2) return;

        Destroy(cards[cards.Count-1].gameObject);

        cards.RemoveAt(cards.Count - 1);

        HandResize(false);
    }

    void HandResize(bool add)
    {
        if(add)
        {
            handContent.sizeDelta = new Vector2(handContent.sizeDelta.x + 115, handContent.sizeDelta.y);
        }
        else
        {
            handContent.sizeDelta = new Vector2(handContent.sizeDelta.x - 115, handContent.sizeDelta.y);
        }

        if(handContent.sizeDelta.x > 800)
        {
            float ratio = 800 / handContent.sizeDelta.x;
            handContent.localScale = new Vector3(ratio, ratio, 1);
        }
    }

    public HandSlot SearchEmptyHandSlot()
    {
        HandSlot slot = null;
        foreach (HandSlot i in cards)
        {
            if (i.deckSlot == null)
            {
                slot = i;
                break;
            }
        }
        return slot;
    }
    #endregion

    #region Card
    public void ClearHand()
    {
        foreach (HandSlot i in cards)
        {
            if (i.deckSlot == null) continue;
            i.Use();
        }

        var tmpLock = 0;

        while(tempHandSlots.Count > 0)
        {
            RemoveTempHandSlot();

            tmpLock++;

            if(tmpLock > 10)
            {
                print("Err");
                break;
            }
        }
    }
    public void AddCard()
    {
        hands++;
        UpdateGroupSynergy();
    }

    #endregion

    #region Temp

    #region TempWeapon

    HandSlot AddTempHandSlot()
    {
        var tempSlot = Instantiate(handSlotPrefab, handContent).GetComponent<HandSlot>();

        tempHandSlots.Add(tempSlot);

        handContent.sizeDelta = new Vector2(handContent.sizeDelta.x + 150, handContent.sizeDelta.y);

        return tempSlot;
    }
    void RemoveTempHandSlot()
    {
        if (tempHandSlots.Count < 1) return;

        Deck.instance.RemoveDeckSlot(tempHandSlots[tempHandSlots.Count - 1].deckSlot);

        tempHandSlots[tempHandSlots.Count - 1].Remove();

        Destroy(tempHandSlots[tempHandSlots.Count - 1].gameObject);
        tempHandSlots.RemoveAt(tempHandSlots.Count - 1);

        handContent.sizeDelta = new Vector2(handContent.sizeDelta.x - 150, handContent.sizeDelta.y);
    }

    public void AddTempWeapon(string weapon)
    {
        var deckSlot = Deck.instance.AddDeckSlot();
        deckSlot.SetSlot(weapon);
        deckSlot.select = true;

        var tempHandSlot = AddTempHandSlot();

        StartCoroutine(CoSetTempHandSlot(tempHandSlot, deckSlot));
    }
    IEnumerator CoSetTempHandSlot(HandSlot tempHandSlot, DeckSlot deckSlot)
    {
        // 이미지를 가져오기 위해 한 번 더 세팅
        yield return null;

        tempHandSlot.Set(deckSlot);
    }

    #endregion

    #region TempUpgrade

    public void ResetTempUpgrade(bool isTurn)
    {
        foreach (HandSlot i in cards)
        {
            i.ResetTempUpgrade(isTurn);
        }
    }

    #endregion

    #endregion

}
