using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SheetMusic;
using static NotePitch;
using static Metrics;


public class KeyboardInput : MonoBehaviour
{
    private readonly IReadOnlyDictionary<KeyCode, NotePitch> keyMappings = new Dictionary<KeyCode, NotePitch>()
    {
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

    private readonly Dictionary<NotePitch, float> startTimes = new();
    private readonly List<Note> notes = new();

    private bool recordingStarted;
    private float tempo;
    private float beat;

    public void BeginRecording(float tempo)
    {
        recordingStarted = true;
        this.tempo = tempo;
        beat = 0;
        notes.Clear();
    }

    public void StopRecording()
    {
        recordingStarted = false;
    }

    /// <summary>
    /// Returns a list of <see cref="Note"/> that are currently being held down.
    /// Each note will have its <see cref="Note.Time"/> set to the beat in this recording
    /// that the note was pressed, and its <see cref="Note.Duration"/> set to how long the
    /// note has been held so far.
    /// </summary>
    /// <returns>A list of <see cref="Note"/></returns>
    public IReadOnlyList<Note> CurrentNotes()
    {
        List<Note> currentNotes = new();
        foreach (NotePitch pitch in startTimes.Keys)
        {
            currentNotes.Add(new Note(pitch, startTimes[pitch], beat - startTimes[pitch]));
        }
        return currentNotes;
    }

    private void Update()
    {
        if (!recordingStarted) return;

        beat += Time.deltaTime * tempo / 60f;

        foreach (KeyCode code in keyMappings.Keys)
        {
            var pitch = keyMappings[code];

            if (Input.GetKeyDown(code))
            {
                startTimes[pitch] = beat;
            }

            if (Input.GetKeyUp(code) && startTimes.TryGetValue(pitch, out float startBeat))
            {
                notes.Add(new Note(pitch, startBeat, beat - startBeat));
            }
        }
    }
}