using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private RectTransform planeTransform;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private float velocityDecay = 2f;

    private float baseVelocity;
    private float velocity;

    private Tween shake;

    public void SetName(string newName)
    {
        nameText.text = newName;
    }

    public void SetBaseVelocity(float velocity)
    {
        baseVelocity = velocity;
    }

    public void AddVelocity(float velocity)
    {
        this.velocity += velocity;
    }

    public void Shake()
    {
        shake.Kill();
        shake = planeTransform
            .DOLocalRotate(new Vector3(0, 0, Random.Range(4, 11)), 0.4f)
            .SetEase(Ease.InOutBounce).SetLoops(2, LoopType.Yoyo);
    }

    private void Awake()
    {
        planeTransform.DOLocalRotate(new Vector3(0, 0, Random.Range(5f, 10f)), 10)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
        planeTransform.DOLocalMoveY(Random.Range(-0.2f, 0.2f), 5f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void LateUpdate()
    {
        transform.localPosition += Vector3.right * Time.deltaTime * baseVelocity;
        transform.localPosition += Vector3.right * Time.deltaTime * velocity;
        velocity = Mathf.Lerp(velocity, baseVelocity, Time.deltaTime * velocityDecay);
    }
}
