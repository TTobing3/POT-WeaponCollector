using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MysteryTooltip : MonoBehaviour
{
    public SlotImageAdapter mysteryIconAdapter;
    public TextMeshProUGUI description;

    RectTransform rect;  // RectTransform ����


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
        // ���콺 �������� ��ũ�� ��ǥ���� ���� ��ǥ�� ��ȯ
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane; // ī�޶� Ŭ���÷��� ���� (������ ī�޶� �տ� ��ġ)

        // ���콺 �������� ���� ��ǥ�� ��ȯ
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // ���� RectTransform�� ���� ��ǥ�� �°� �̵�
        rect.position = worldPos;

    }
}
