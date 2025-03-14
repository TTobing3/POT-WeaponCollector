using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffTooltip : MonoBehaviour
{
    public SlotImageAdapter buffIconAdapter;
    public TextMeshProUGUI description;

    RectTransform rect;  // RectTransform ����

    private Vector2 pivotOffset = new Vector2(0f, 0f);  // �⺻ �Ǻ�

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
        // ���콺 �������� ��ũ�� ��ǥ���� ���� ��ǥ�� ��ȯ
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane; // ī�޶� Ŭ���÷��� ���� (������ ī�޶� �տ� ��ġ)

        // ���콺 �������� ���� ��ǥ�� ��ȯ
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // ���� RectTransform�� ���� ��ǥ�� �°� �̵�
        rect.position = worldPos;

        // ������ ũ��� ȭ�� ũ�⸦ ���� �Ǻ��� ���� (������ ȭ���� ����� �ʰ�)
        AdjustPivotToFitScreen(mousePos);
    }
    void AdjustPivotToFitScreen(Vector2 mousePos)
    {
        // ���� ȭ�� ũ�⸦ ��ũ�� ��ǥ�� ������
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 tooltipSize = rect.sizeDelta;  // ������ ũ��

        Vector2 newPivot = pivotOffset;  // �⺻ �Ǻ� ������

        // ������ ȭ�� ������ ���� ���
        if (mousePos.x + tooltipSize.x * 2/ 2 > screenSize.x)
            newPivot.x = 1.0f;  // �Ǻ��� �������� ����

        /*
        // ���� ȭ�� ������ ���� ���
        if (mousePos.x - tooltipSize.x / 2 < 0)
            newPivot.x = 0.0f;  // �Ǻ��� ���������� ����
        // ���� ȭ�� ������ ���� ���
        if (mousePos.y + tooltipSize.y / 2 > screenSize.y)
            newPivot.y = 1.0f;  // �Ǻ��� �Ʒ������� ����

        // �Ʒ��� ȭ�� ������ ���� ���
        if (mousePos.y - tooltipSize.y / 2 < 0)
            newPivot.y = 0.0f;  // �Ǻ��� �������� ����
        */
        // ���ο� �Ǻ� ����
        rect.pivot = newPivot;
        rect.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
    }

}
