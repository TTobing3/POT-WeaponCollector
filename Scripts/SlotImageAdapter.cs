using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;

public class SlotImageAdapter : MonoBehaviour
{
    Image image;
    SpriteRenderer spriteRenderer;
    SpriteResolver spriteResolver;
    RectTransform rect;

    string category, item;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        spriteResolver = GetComponent<SpriteResolver>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //이미지 바꾸기
    public void ImageChange(string category, string item, bool resize = true)
    {
        if (item == "None") gameObject.SetActive(false);
        if (image != null) image.enabled = false;

        this.category = category;
        this.item = item;

        spriteRenderer.sprite = null;
        spriteResolver.SetCategoryAndLabel(category, item);

        StartCoroutine(CoSpriteChange(resize));
    }

    IEnumerator CoSpriteChange(bool resize = true)
    {
        yield return null;

        if(image != null)
        {
            image.enabled = true;

            image.sprite = spriteRenderer.sprite;

            if (resize)
                Resize();
        }
    }

    //사이즈 적응형
    public void Resize()
    {
        image.SetNativeSize();
        var size = image.sprite.rect.size;
        var tmpSize = size.x > size.y ? new Vector2(1, size.y / size.x) : new Vector2(size.x / size.y, 1);
        tmpSize = Mathf.Abs(tmpSize.x - tmpSize.y) < 0.2f ? new Vector2(tmpSize.x - 0.4f, tmpSize.y - 0.4f) : tmpSize;
        rect.GetComponent<RectTransform>().localScale = tmpSize;
    }

}
