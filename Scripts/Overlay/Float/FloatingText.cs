using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class FloatData
{
    public string contents;
    public float fontSize;
    public float time;
    public float[] direction = new float[] { 0, 150 };
    public Vector3 pos;
    public Color color = Color.white;

    public FloatData(string contents, Vector3 pos, float time = 1)
    {
        this.contents = contents;
        this.time = time;
        this.pos = pos;
    }

    public FloatData SetFontsize(float fontSize = 64)
    {
        this.fontSize = fontSize;

        return this;
    }

    public FloatData SetDirection(float[] direction)
    {
        this.direction = direction;

        return this;
    }

    public FloatData SetColor(Color color)
    {
        this.color = color;

        return this;
    }
}

public class FloatingText : MonoBehaviour
{
    RectTransform rect;
    TextMeshProUGUI text;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponent<TextMeshProUGUI>();
    }

    public void Float(FloatData floatData)
    {
        DOTween.Kill(rect);
        DOTween.Kill(text);

        var time = floatData.time; //Random.Range(1, 2f);

        rect.anchoredPosition = floatData.pos;

        text.color = floatData.color;
        text.text = floatData.contents;
        text.fontSize = floatData.fontSize;

        var dirX = Random.Range(floatData.direction[0] - 25, floatData.direction[0] + 25);
        var dirY = Random.Range(floatData.direction[1] - 50, floatData.direction[1] + 50);

        rect.DOAnchorPosX(floatData.pos.x + dirX, time);
        rect.DOAnchorPosY(floatData.pos.y + dirY, time);
        text.DOFade(0, time).SetEase(Ease.InCubic).OnComplete(()=> gameObject.SetActive(false) );
    }
}
