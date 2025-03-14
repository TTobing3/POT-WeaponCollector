using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UsableSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public GameObject dragPreviewPrefab; // �巡�� ������� ����� ������
    GameObject dragPreviewInstance; // �巡�� �� �����Ǵ� ������ �ν��Ͻ�
    RectTransform dragPreviewRectTransform;

    Canvas canvas; // Canvas�� �����ͼ� UI ��ġ�� ������ �� ���

    DeckSlot deckSlot;

    private void Start()
    {
        canvas = UIManager.instance.canvases[2]; // �������� �ٷ�� ĵ����
        deckSlot = GetComponent<DeckSlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CheckUsable()) return;

        #region System

        // �巡�� ������ �ν��Ͻ� ����
        dragPreviewInstance = Instantiate(dragPreviewPrefab, canvas.transform);
        dragPreviewRectTransform = dragPreviewInstance.GetComponent<RectTransform>();

        // �巡�� �������� �̹��� ���� (���� �������� �̹����� ����)
        Image originalImage = deckSlot.itemImage;
        Image previewImage = dragPreviewInstance.GetComponent<Image>();
        previewImage.sprite = originalImage.sprite;

        // �巡�� �����䰡 Ŭ���� �����۰� ���� ��ġ���� ����
        dragPreviewRectTransform.position = transform.position;

        #endregion

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CheckUsable()) return;

        #region System

        // �巡�� �����䰡 ���콺�� ���� ������
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out mousePosition);

        dragPreviewRectTransform.anchoredPosition = mousePosition;

        #endregion
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        #region System

        // �巡�װ� ������ ������ ����
        if (dragPreviewInstance != null)
        {
            Destroy(dragPreviewInstance);
        }

        #endregion
    }

    bool CheckUsable()
    {
        if (deckSlot.select) return false;
        if (deckSlot.GetWeaponData.number == -1) return false;

        return true;
    }
}
