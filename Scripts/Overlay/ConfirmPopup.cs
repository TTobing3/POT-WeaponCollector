using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmPopup : MonoBehaviour
{
    RectTransform rect;

    public TextMeshProUGUI title, description;
    public Button confirmButton, cancleButton;

    System.Action action;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        confirmButton.onClick.AddListener(Confirm);
        cancleButton.onClick.AddListener(Cancle);
    }

    public void Set(string title, string description, System.Action action)
    {
        this.title.text = title;
        this.description.text = description;
        this.action = action;

        StartCoroutine(CoSetSize());
    }

    IEnumerator CoSetSize()
    {
        yield return null;

        rect.sizeDelta = new Vector2(rect.sizeDelta.x,
            30 + 100 + 30 + description.GetComponent<RectTransform>().sizeDelta.y + 30 + 150 + 50
            );
    }

    void Confirm()
    {
        if(action != null)
            action();

        action = null;

        gameObject.SetActive(false);
    }

    void Cancle()
    {
        gameObject.SetActive(false);
    }
}
