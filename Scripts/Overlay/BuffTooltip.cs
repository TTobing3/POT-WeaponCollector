using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffTooltip : MonoBehaviour
{
    public SlotImageAdapter buffIconAdapter;
    public TextMeshProUGUI description;

    RectTransform rect;  // RectTransform 참조

    private Vector2 pivotOffset = new Vector2(0f, 0f);  // 기본 피봇

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Set(string buff)
    {
        gameObject.SetActive(true);

        buffIconAdapter.ImageChange("Buff", buff, false);
        description.text =  $"{buff} : {DataManager.instance.AllBuffDatas[buff].description}";

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

        // 툴팁의 크기와 화면 크기를 비교해 피봇을 조정 (툴팁이 화면을 벗어나지 않게)
        AdjustPivotToFitScreen(mousePos);
    }
    void AdjustPivotToFitScreen(Vector2 mousePos)
    {
        // 현재 화면 크기를 스크린 좌표로 가져옴
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 tooltipSize = rect.sizeDelta;  // 툴팁의 크기

        Vector2 newPivot = pivotOffset;  // 기본 피봇 오프셋

        // 오른쪽 화면 밖으로 나갈 경우
        if (mousePos.x + tooltipSize.x * 2/ 2 > screenSize.x)
            newPivot.x = 1.0f;  // 피봇을 왼쪽으로 변경

        /*
        // 왼쪽 화면 밖으로 나갈 경우
        if (mousePos.x - tooltipSize.x / 2 < 0)
            newPivot.x = 0.0f;  // 피봇을 오른쪽으로 변경
        // 위쪽 화면 밖으로 나갈 경우
        if (mousePos.y + tooltipSize.y / 2 > screenSize.y)
            newPivot.y = 1.0f;  // 피봇을 아래쪽으로 변경

        // 아래쪽 화면 밖으로 나갈 경우
        if (mousePos.y - tooltipSize.y / 2 < 0)
            newPivot.y = 0.0f;  // 피봇을 위쪽으로 변경
        */
        // 새로운 피봇 적용
        rect.pivot = newPivot;
        rect.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
    }

}
