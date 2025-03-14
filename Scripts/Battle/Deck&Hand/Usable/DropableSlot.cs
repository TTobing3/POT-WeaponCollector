using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropableSlot : MonoBehaviour, IDropHandler
{
    public DeckSlot deckSlot; // �� ������ ���� ����
    public HandSlot handSlot;

    private void Awake()
    {
        deckSlot = GetComponent<DeckSlot>();
        handSlot = GetComponent<HandSlot>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // �巡�׵� �������� ������ ������
        GameObject draggedItem = eventData.pointerDrag;

        if (draggedItem == null) return;
        if (draggedItem.GetComponent<DeckSlot>() == null || draggedItem.GetComponent<DeckSlot>().GetWeaponData.number == -1) return;

        #region ���� ����

        if (handSlot != null && handSlot.deckSlot == null)
        {
            EquipmentWeapon(draggedItem);
        }

        #endregion

        #region �Ҹ�ǰ ���

        if (deckSlot != null)
        {
            // �ֹ��� ���
            if (deckSlot.GetWeaponData.number != -1 
                && !deckSlot.GetWeaponData.type.Contains("�ֹ���")
                && draggedItem.GetComponent<DeckSlot>().GetWeaponData.type.Contains("�ֹ���"))
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

        Debug.Log($"{draggedItem.GetComponent<DeckSlot>().GetWeaponData.name}�� {deckSlot.GetWeaponData.name}�� ���� �Ű���ϴ�.");

        deckSlot.SwapSlot(draggedItem.GetComponent<DeckSlot>());

    }

    void EquipmentWeapon(GameObject draggedItem)
    {
        draggedItem.GetComponent<DeckSlot>().SelectSlot(true, handSlot);
    }

    void ApplyScrollToWeapon(GameObject draggedItem)
    {
        // �ֹ����� ���� ������ �ɷ�ġ�� ����Ű�� ����
        // dragged <- �ֹ��� / deckSlot <- ����

        Debug.Log($"{draggedItem.GetComponent<DeckSlot>().GetWeaponData.name}�� {deckSlot.GetWeaponData.name}�� ���Ǿ����ϴ�.");

        deckSlot.TryUpgrade(draggedItem.GetComponent<DeckSlot>().GetWeaponData);
        draggedItem.GetComponent<DeckSlot>().DropSlot();
    }
}
