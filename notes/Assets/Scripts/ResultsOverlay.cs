using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultsOverlay : MonoBehaviour
{
    public TextMeshProUGUI userText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI performanceText;
    public Button button;

    public CanvasGroup cg;
    private Tween cgTween;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            NetworkClient.Shutdown();
            NetworkServer.Shutdown();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        SetVisibility(false, false);
    }

    public void SetVisibility(bool visible, bool animate = true)
    {
        cgTween.Kill();
        float alpha = visible ? 1 : 0;
        if (animate)
        {
            cgTween = cg.DOFade(alpha, 0.7f);
        }
        else
        {
            cg.alpha = alpha;
        }
    }
}
