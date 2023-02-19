using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem; 
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
    public IReadOnlyList<Note> Notes => notes;

    private bool recordingStarted;
    private float tempo;
    private float beat;

    /// <summary>
    /// The beat we are currently on in the input recording.
    /// </summary>
    public float Beat => beat;

    public void BeginRecording(float tempo, float startBeat = 0)
    {
        recordingStarted = true;
        this.tempo = tempo;
        beat = startBeat;
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


    private void Start()
    {
        InputSystem.onDeviceChange += (device, change) =>
        {

            if (change != InputDeviceChange.Added) return;

            var midiDevice = device as Minis.MidiDevice;
            if (midiDevice == null) return;

            midiDevice.onWillNoteOn += (note, velocity) => {
                // Note that you can't; use note.velocity because the state
                // hasn't been updated yet (as this is "will" event). The note
                // object is only useful to specify the target note (note
                // number, channel number, device name, etc.) Use the velocity
                // argument as an input note velocity.
                Debug.Log(string.Format(
                    "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    velocity,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));

                startTimes[(NotePitch)note.noteNumber] = beat;
            };

            midiDevice.onWillNoteOff += (note) => {
                Debug.Log(string.Format(
                    "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));

                if (startTimes.TryGetValue((NotePitch)note.noteNumber, out float startBeat))
                {
                    notes.Add(new Note((NotePitch)note.noteNumber, startBeat, beat - startBeat));
                }
            };
        };

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