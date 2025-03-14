using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipOff : MonoBehaviour
{
    private void OnDisable()
    {
        Tooltip.instance.OffTooltip();
    }
}
