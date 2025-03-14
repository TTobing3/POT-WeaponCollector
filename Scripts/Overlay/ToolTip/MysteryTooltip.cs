using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MysteryTooltip : MonoBehaviour
{
    public SlotImageAdapter mysteryIconAdapter;
    public TextMeshProUGUI description;

    RectTransform rect;  // RectTransform 참조


    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Set(string mystery)
    {
        gameObject.SetActive(true);

        mysteryIconAdapter.ImageChange("Mystery", mystery, false);
        description.text = $"{mystery} : {DataManager.instance.AllMysteryDatas[mystery].description}";

        StartCoroutine(CoResize());
    }

    IEnumerator CoResize()
    {
        yield return null;

        rect.sizeDelta = new Vector2(GetComponentInChildren<TextMeshProUGUI>().GetComponent<RectTransform>().sizeDelta.x + 120, rect.sizeDelta.y);
    }

    private void Update()
    {
        FollowMouse();
    }

    void FollowMouse()
    {
        // 마우스 포지션을 스크린 좌표에서 월드 좌표로 변환
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane; // 카메라 클립플레인 설정 (툴팁을 카메라 앞에 배치)

        // 마우스 포지션을 월드 좌표로 변환
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // 툴팁 RectTransform을 월드 좌표에 맞게 이동
        rect.position = worldPos;

    }
}
