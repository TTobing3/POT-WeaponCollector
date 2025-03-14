using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Fade : MonoBehaviour
{
    public static Fade instance;

    Image image;

    void Awake()
    {
        if (instance == null)
            instance = this;

        image = GetComponent<Image>();
    }

    public void FadeAction(System.Action action)
    {
        image.DOFade(1, 0.5f).OnComplete(() =>
        {
            action();
            image.DOFade(0, 0.5f);
        });
    }
}
