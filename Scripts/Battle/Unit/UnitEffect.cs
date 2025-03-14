using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using DG.Tweening;

public class UnitEffect : MonoBehaviour
{
    public Material paintWhite;
    public SpriteRenderer effectRenderer;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 피격 이펙트
    public void HitEffect(string effect = "타격")
    {
        effectRenderer.gameObject.SetActive(true);
        effectRenderer.GetComponent<SpriteResolver>().SetCategoryAndLabel("Effect", effect);
        effectRenderer.color = Color.white;
        effectRenderer.DOFade(0, 0.5f).OnComplete(()=> {
            effectRenderer.gameObject.SetActive(false);
        });
    }

    public void HitWhiteFlash()
    {
        StartCoroutine(CoHitWhiteFlash());
    }

    IEnumerator CoHitWhiteFlash()
    {
        Material originMaterial = spriteRenderer.material;
        spriteRenderer.material = paintWhite;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.material = originMaterial;
    }
}
