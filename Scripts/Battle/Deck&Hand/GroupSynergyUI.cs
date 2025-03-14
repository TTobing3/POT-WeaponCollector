using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 컴포넌트를 위해 필요
using UnityEngine.U2D.Animation; // SpriteResolver를 사용하기 위해 필요
using TMPro;
public class GroupSynergyUI : MonoBehaviour
{
    public TextMeshProUGUI nameText, countText; 
    public Image iconImage;
    public SlotImageAdapter slotImageAdapter;
    public Tooltipable tooltipable;
    void Awake()
    {
        slotImageAdapter = iconImage.GetComponent<SlotImageAdapter>();
        tooltipable = GetComponent<Tooltipable>();
    }
    public void Set(GroupSynergy synergy)
    {
        nameText.text = synergy.name;
        countText.text = synergy.count.ToString();
        slotImageAdapter.ImageChange("GroupIcon", synergy.name, false);
        tooltipable.data = $"GroupSynergy:{synergy.name}";
    }
}
