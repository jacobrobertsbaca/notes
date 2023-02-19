using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SheetMusic;
using static NotePitch;


public class KeyboardInput : MonoBehaviour
{

    public List<Note> NotesPlayed = new List<Note>();
    float StartTime = 0;

    /// <summary>
    /// List of all keys that are mapped to notes.
    /// Specifically the home row of virtualpiano.net
    /// </summary>
    public List<KeyCode> NoteKeyCodes = new List<KeyCode>()
    {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K,
        KeyCode.L, KeyCode.R, KeyCode.E,
        KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.P
    };

    public IDictionary<KeyCode, NotePitch> KeyCodeToPitch = new Dictionary<KeyCode, NotePitch>() {
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


    
    // handle for unity keycodes 

    void Update()
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
                Debug.Log((Time.time - StartTime).ToString("00:00.00"));
                float noteDuration = Time.time - StartTime; 
                NotesPlayed.Add(new Note(KeyCodeToPitch[tempkey], StartTime, noteDuration, 20));
                foreach (Note tempNote in NotesPlayed)
                {
                    Debug.Log("Here is a note you played: " + (NotePitch) tempNote.Pitch);
                    Debug.Log("Here is the start time of the note " + tempNote.Time);
                    Debug.Log("Here is the beat duration" + tempNote.Duration); 
                }
            }
        }
    }
}