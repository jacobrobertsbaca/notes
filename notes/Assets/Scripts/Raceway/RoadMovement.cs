using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoadMovement : MonoBehaviour
{
    private float speed = -200;
    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
    // transform.Translate(Vector3.forward * Time.deltaTime * speed);
    Vector2 position = rectTransform.anchoredPosition;
 
     position.x += speed * Time.deltaTime;
     
     rectTransform.anchoredPosition = position;
     if (position.x < 0)
     {
        
     }

    }
    // private float speed = -200;
    // private RectTransform rectTransform;
    // void Start()
    // {
    //     rectTransform = GetComponent<RectTransform>();
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
    // // transform.Translate(Vector3.forward * Time.deltaTime * speed);
    // Vector2 position = rectTransform.anchoredPosition;
 
    //  position.x += speed * Time.deltaTime;
     
    //  rectTransform.anchoredPosition = position;
    //  //if (position.x <)
    // }
}
