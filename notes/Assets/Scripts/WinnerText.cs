using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinnerText : MonoBehaviour
{
    public TextMeshProUGUI userText;
    public CanvasGroup cg;
    private Tween cgTween;

    private void Awake()
    {
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

    public void SetUsername(string username)
    {
        userText.text = username;
    }
}
