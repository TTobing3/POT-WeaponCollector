using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropableSlot : MonoBehaviour, IDropHandler
{
    public DeckSlot deckSlot; // 이 슬롯의 무기 정보
    public HandSlot handSlot;

    private void Awake()
    {
        deckSlot = GetComponent<DeckSlot>();
        handSlot = GetComponent<HandSlot>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 드래그된 아이템의 정보를 가져옴
        GameObject draggedItem = eventData.pointerDrag;

        if (draggedItem == null) return;
        if (draggedItem.GetComponent<DeckSlot>() == null || draggedItem.GetComponent<DeckSlot>().GetWeaponData.number == -1) return;

        #region 무기 장착

        if (handSlot != null && handSlot.deckSlot == null)
        {
            EquipmentWeapon(draggedItem);
        }

        #endregion

        #region 소모품 사용

        if (deckSlot != null)
        {
            // 주문서 사용
            if (deckSlot.GetWeaponData.number != -1 
                && !deckSlot.GetWeaponData.type.Contains("주문서")
                && draggedItem.GetComponent<DeckSlot>().GetWeaponData.type.Contains("주문서"))
            {
                ApplyScrollToWeapon(draggedItem);
                return;
            }

            MoveSlot(draggedItem);
        }

        #endregion



    }

    void MoveSlot(GameObject draggedItem)
    {
        if (deckSlot.select || draggedItem.GetComponent<DeckSlot>().select) return;

        Debug.Log($"{draggedItem.GetComponent<DeckSlot>().GetWeaponData.name}를 {deckSlot.GetWeaponData.name}쪽 으로 옮겼습니다.");

        deckSlot.SwapSlot(draggedItem.GetComponent<DeckSlot>());

    }

    void EquipmentWeapon(GameObject draggedItem)
    {
        draggedItem.GetComponent<DeckSlot>().SelectSlot(true, handSlot);
    }

    void ApplyScrollToWeapon(GameObject draggedItem)
    {
        // 주문서에 따라 무기의 능력치를 향상시키는 로직
        // dragged <- 주문서 / deckSlot <- 무기

        Debug.Log($"{draggedItem.GetComponent<DeckSlot>().GetWeaponData.name}가 {deckSlot.GetWeaponData.name}에 사용되었습니다.");

        deckSlot.TryUpgrade(draggedItem.GetComponent<DeckSlot>().GetWeaponData);
        draggedItem.GetComponent<DeckSlot>().DropSlot();
    }
}
