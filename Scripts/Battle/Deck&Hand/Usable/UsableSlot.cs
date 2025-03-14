using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UsableSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public GameObject dragPreviewPrefab; // 드래그 프리뷰로 사용할 프리팹
    GameObject dragPreviewInstance; // 드래그 중 생성되는 프리뷰 인스턴스
    RectTransform dragPreviewRectTransform;

    Canvas canvas; // Canvas를 가져와서 UI 위치를 조정할 때 사용

    DeckSlot deckSlot;

    private void Start()
    {
        canvas = UIManager.instance.canvases[2]; // 오버레이 다루는 캔버스
        deckSlot = GetComponent<DeckSlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CheckUsable()) return;

        #region System

        // 드래그 프리뷰 인스턴스 생성
        dragPreviewInstance = Instantiate(dragPreviewPrefab, canvas.transform);
        dragPreviewRectTransform = dragPreviewInstance.GetComponent<RectTransform>();

        // 드래그 프리뷰의 이미지 설정 (실제 아이템의 이미지를 복사)
        Image originalImage = deckSlot.itemImage;
        Image previewImage = dragPreviewInstance.GetComponent<Image>();
        previewImage.sprite = originalImage.sprite;

        // 드래그 프리뷰가 클릭된 아이템과 같은 위치에서 시작
        dragPreviewRectTransform.position = transform.position;

        #endregion

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CheckUsable()) return;

        #region System

        // 드래그 프리뷰가 마우스를 따라 움직임
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

        // 드래그가 끝나면 프리뷰 제거
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
