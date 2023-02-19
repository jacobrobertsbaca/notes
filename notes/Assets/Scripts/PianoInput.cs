using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using static SheetMusic; 

sealed class PianoInput : MonoBehaviour
{

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


    public IReadOnlyList<Note> CurrentNotes()
    {
        List<Note> currentNotes = new();
        foreach (NotePitch pitch in startTimes.Keys)
        {
            currentNotes.Add(new Note(pitch, startTimes[pitch], beat - startTimes[pitch]));
        }
        return currentNotes;
    }

    void Update()
    {
        if (!recordingStarted) return;

        beat += Time.deltaTime * tempo / 60f;

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

                startTimes[(NotePitch) note.noteNumber] = beat; 
            };

            midiDevice.onWillNoteOff += (note) => {
                Debug.Log(string.Format(
                    "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                    note.noteNumber,
                    note.shortDisplayName,
                    (note.device as Minis.MidiDevice)?.channel,
                    note.device.description.product
                ));

                if (startTimes.TryGetValue((NotePitch) note.noteNumber, out float startBeat))
                {
                    notes.Add(new Note((NotePitch) note.noteNumber, startBeat, beat - startBeat));
                }
            };
        };

    }
}