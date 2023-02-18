using System.Collections;
using System.Collections.Generic;
using MidiParser;
using UnityEngine;

public class Playground : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string kStreamingAssetsFolder = Application.streamingAssetsPath;
        var midiFile = new MidiFile(kStreamingAssetsFolder + "/twinkle.mid");

        // 0 = single-track, 1 = multi-track, 2 = multi-pattern
        var midiFileformat = midiFile.Format;

        // also known as pulses per quarter note
        var ticksPerQuarterNote = midiFile.TicksPerQuarterNote;

        foreach (var track in midiFile.Tracks)
        {
            foreach (var midiEvent in track.MidiEvents)
            {
                if (midiEvent.MidiEventType == MidiEventType.NoteOn)
                {
                    var channel = midiEvent.Channel;
                    var note = midiEvent.Note;
                    var velocity = midiEvent.Velocity;
                    Debug.Log(channel);
                    Debug.Log(note);
                    Debug.Log(velocity);
                    Debug.Log($"Channel: {channel}");
                }
            }

            foreach (var textEvent in track.TextEvents)
            {
                if (textEvent.TextEventType == TextEventType.Lyric)
                {
                    var time = textEvent.Time;
                    var text = textEvent.Value;
                }
            }
        }
    }
}
