using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }
    public Transform Target { get; set; }

    [SerializeField] private float zoomMultiplier;

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Update()
    {
        if (!Target) return;
        transform.position = Vector3.Lerp(transform.position, Target.position, zoomMultiplier * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }
}
