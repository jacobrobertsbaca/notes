using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static SheetMusic; 

/// <summary>
/// Analagous to the KeyBoard Input Class
/// Except its a fuckin piano boiiii
/// </summary>
public class NewBehaviourScript : MonoBehaviour
{

    public float Tempo { get; set; }

    public SheetMusic Music { get;  }

    public List<Note> NotesPlayed { get; } = new List<Note>();

    public float StartTime { get; set; }

    public bool gameStarted { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
