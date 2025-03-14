using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameOverManager : MonoBehaviour
{
    public Image fade, background;
    public Button gameOverButton;
    public TextMeshProUGUI gameOverText;

    private void OnEnable()
    {
        gameOverButton.onClick.AddListener(GameOver);
    }

    public void Set()
    {
        gameObject.SetActive(true);

        background.color = Color.clear;
        gameOverText.color = Color.clear;

        gameOverButton.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        gameOverButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;

        //

        background.DOColor(new Color(0, 0, 0, 0.5f), 0.5f).OnComplete(() =>
        {
            gameOverText.DOColor(new Color(1, 1, 1, 1), 0.5f);
        });

        gameOverButton.GetComponent<RectTransform>().DOSizeDelta(new Vector2(480, 180), 0.5f).OnComplete(()=> 
        {
            gameOverButton.GetComponentInChildren<TextMeshProUGUI>().DOColor(new Color(1, 1, 1, 1), 0.5f);
        });

        //

        Deck.instance.MoveOutDeck();

        //

        UIManager.instance.playerUI.SetActive(false);
    }

    public void GameOver()
    {
        fade.color = Color.clear;
        fade.gameObject.SetActive(true);
        fade.DOFade(1, 1).OnComplete(() => { SceneManager.LoadScene("StartScene"); });
    }
}
