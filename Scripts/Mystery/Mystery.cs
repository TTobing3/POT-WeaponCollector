using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Mystery : MonoBehaviour
{
    Tooltipable tooltipable;

    SlotImageAdapter mysteryImageAdapter;

    public string mystery;

    void Awake()
    {
        tooltipable = GetComponent<Tooltipable>();
        mysteryImageAdapter = GetComponent<SlotImageAdapter>();
    }
    public void Set(string mystery)
    {
        GetComponent<Image>().color = new Color(1,1,1,0);
        GetComponent<Image>().DOFade(1, 1);
        tooltipable.data = $"Mystery:{mystery}";
        this.mystery = mystery;
        mysteryImageAdapter.ImageChange("Mystery", mystery, false);
    }
}
