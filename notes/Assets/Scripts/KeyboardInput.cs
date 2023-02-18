using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SheetMusic;


public class KeyboardInput : MonoBehaviour
{

    public List<Note> Notes;
    float StartTime = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Amount of time Pressed");
            Debug.Log((Time.time - StartTime).ToString("00:00.00"));
        }
    }
}
