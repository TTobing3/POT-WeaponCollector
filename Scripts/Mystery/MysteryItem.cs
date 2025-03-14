using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MysteryItem : MonoBehaviour
{
    public MysteryData mysteryData;

    RectTransform rect, itemRect;
    Button itemButton;
    SlotImageAdapter slotImageAdapter;

    public int number;

    bool selected = false;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        itemButton = transform.GetChild(0).GetComponent<Button>();
        itemRect = transform.GetChild(0).GetComponent<RectTransform>();
        slotImageAdapter = transform.GetChild(0).GetComponent<SlotImageAdapter>();

        itemButton.onClick.AddListener(Select);
    }

    public void Set(MysteryData mysteryData)
    {
        this.mysteryData = mysteryData;

        slotImageAdapter.ImageChange("Mystery", mysteryData.name, false);
    }

    public void Select()
    {
        if(selected)
        {
            MysteryManager.instance.Finish();
            Float(false);

            UIManager.instance.AddMystery(mysteryData.name);
            StageManager.instance.mysteries.Add(mysteryData);
        }
        else
        {
            MysteryManager.instance.itemSelect(number);
        }
    }

    public void Float(bool selected)
    {
        DOTween.Kill(itemRect);
        this.selected = selected;

        if(selected)
        {
            itemRect.DOAnchorPosY(300, 1);
        }
        else
        {
            itemRect.DOAnchorPosY(120, 1);
        }
    }
}
