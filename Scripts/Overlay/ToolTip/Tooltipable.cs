using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltipable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Weapon : name,level,rune1,rune2 ...
    public string data = "";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Tooltip.instance != null)
        {
            if(eventData.pointerEnter.GetComponent<DeckSlot>() != null)
            {
                Tooltip.instance.Set(eventData.pointerEnter.GetComponent<DeckSlot>());
                return;
            }
            if (eventData.pointerEnter.GetComponent<HandSlot>() != null)
            {
                Tooltip.instance.Set(eventData.pointerEnter.GetComponent<HandSlot>().deckSlot);
                return;
            }

            Tooltip.instance.Set(true, data);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Tooltip.instance != null)
        {
            Tooltip.instance.Set(false, data);
        }
    }
}
