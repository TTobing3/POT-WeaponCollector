using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyInterect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Enemy enemy;

    Material spriteMaterial;

    public bool focus;

    private void Awake()
    {
        spriteMaterial = enemy.GetComponent<SpriteRenderer>().material;
    }

    void OnDisable()
    {
        Tooltip.instance.OffTooltip();
    }

    #region Focus

    public void FocusOn(bool isFocus)
    {
        if (isFocus)
        {
            spriteMaterial.SetFloat("_OutlineAlpha", 1);
            spriteMaterial.SetColor("_OutlineColor", new Color(1, 0.5f, 0));
        }
        else
        {
            spriteMaterial.SetFloat("_OutlineAlpha", 0);
            spriteMaterial.SetColor("_OutlineColor", Color.white);

            TurnManager.instance.enemyFocus = false;
        }
    }

    public void KeepFocus()
    {
        if (!focus) return;

        TurnManager.instance.enemyFocus = true;
        FocusOn(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TurnManager.instance.turn[1] != 1 || TurnManager.instance.interectLock) return;

        FocusOn(true);
        focus = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FocusOn(false);
        focus = false;
    }

    #endregion

}
