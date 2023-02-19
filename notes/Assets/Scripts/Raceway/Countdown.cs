using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup cg;

    private void Awake()
    {
        cg.alpha = 0f;
    }

    public void StartCountdown(Action onComplete)
    {
        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => cg.alpha = 0);
        
        for (int i = 3; i >= 0; i--)
        {
            string t = i == 0 ? "Go!" : i.ToString();
            if (i == 0) s.AppendCallback(() => onComplete?.Invoke());
            s.AppendCallback(() => text.text = t)
            .Append(cg.DOFade(1f, 0.5f).SetLoops(2, LoopType.Yoyo));
        }
    }
}
