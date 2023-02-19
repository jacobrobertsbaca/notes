using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private RectTransform planeTransform;
    [SerializeField] private TextMeshProUGUI nameText;

    private void Awake()
    {
        planeTransform.DOLocalRotate(new Vector3(0, 0, Random.Range(5f, 10f)), 10)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
        planeTransform.DOLocalMoveY(Random.Range(-0.2f, 0.2f), 5f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
