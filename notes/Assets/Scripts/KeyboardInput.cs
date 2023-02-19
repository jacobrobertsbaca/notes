using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SheetMusic;
using static NotePitch;
using static Metrics;



public class KeyboardInput: MonoBehaviour
{

    public float Tempo { get; set;  }

    public SheetMusic Music { get;  }

    public float GroundTime { get; set; }

    public List<Note> NotesPlayed { get; } = new List<Note>();

    public float StartTime { get; set; }

    public bool gameStarted { get; set; }

    private static readonly IReadOnlyDictionary<KeyCode, NotePitch> KeyCodeToPitch = new Dictionary<KeyCode, NotePitch>() {
        {KeyCode.A, B4},
        {KeyCode.S, C5},
        {KeyCode.D, D5},
        {KeyCode.F, E5},
        {KeyCode.G, F5},
        {KeyCode.H, G5},
        {KeyCode.J, A5},
        {KeyCode.K, B5},
        {KeyCode.L, C6},
        {KeyCode.R, Eb5},
        {KeyCode.E, Db5},
        {KeyCode.Y, Gb5},
        {KeyCode.U, Ab5},
        {KeyCode.I, Bb5},
        {KeyCode.P, B5},
    };

    private static readonly List<KeyCode> NoteKeyCodes = new List<KeyCode>()
    {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K,
        KeyCode.L, KeyCode.R, KeyCode.E,
        KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.P
};

    void Update()
    {
        // if no space do not start recording input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameStarted = true;
        }

        if (gameStarted)
        {

            foreach (KeyCode tempkey in NoteKeyCodes)
            {
                if (Input.GetKeyDown(tempkey))
                {
                    StartTime = Time.time;
                }

                if (Input.GetKeyUp(tempkey))
                {
                    Debug.Log("Amount of time Pressed for key: " + tempkey);
                    float noteDuration = (Time.time - StartTime) * (Tempo / 60);
                    Debug.Log("This is the TEMPO" + Tempo);
                    NotePitch notePitch = KeyCodeToPitch[tempkey];
                    NotesPlayed.Add(new Note(notePitch, StartTime, noteDuration, 20));
                    Debug.Log("Duration: " + noteDuration + ", " + "Start Time: " + StartTime + ", " + "Note Pitch: " + notePitch);
                }
            }
        }
    }

}