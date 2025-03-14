using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Buff : MonoBehaviour
{
    Tooltipable tooltipable;
    [SerializeField] TextOutLine textOutLine;

    public SlotImageAdapter buffImageAdapter;
    public TextMeshProUGUI countText;

    public string buff;

    void Awake()
    {
        tooltipable = GetComponent<Tooltipable>();
    }
    public void Set(string buff, string count)
    {
        tooltipable.data = $"Buff:{buff}";
        this.buff = buff;
        buffImageAdapter.ImageChange("Buff", buff, false);

        count = int.Parse(count) < 1 ? "" : count;
        countText.text = count;
    }

    public void UpdateCount(string count)
    {
        count = int.Parse(count) < 1 ? "" : count;
        countText.text = count;
    }
}
