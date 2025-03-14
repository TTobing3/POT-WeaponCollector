using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BuffLine : MonoBehaviour
{
    public SlotImageAdapter buffImageAdapter;
    public TextMeshProUGUI nameText;

    public void Set(string buff)
    {
        buffImageAdapter.ImageChange("Buff", buff, false);
        nameText.text = buff;
    }
}
