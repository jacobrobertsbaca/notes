using System;
using System.Collections;
using System.Collections.Generic;
using MidiParser;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using static NotePitch;
using static SheetMusic;

public class Playground : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">Must be in the "StreamingAssets" directory.</param>
    /// <param name="debug"></param>
    /// <returns></returns>
    public SheetMusic getDetails(string fileName, bool debug = false)
    {
        string kStreamingAssetsFolder = Application.streamingAssetsPath;
        var midiFile = new MidiFile(kStreamingAssetsFolder + fileName);

        // NOTE: Optional features
        var midiFileformat = midiFile.Format;                                                 // 0 = single-track, 1 = multi-track, 2 = multi-pattern
        var ticksPerQuarterNote = midiFile.TicksPerQuarterNote;                               // also known as pulses per quarter note

        // #############################################################################
        // ################################# META-DATA #################################
        // #############################################################################

        var metaData = midiFile.Tracks[0].MidiEvents;
        //Assert.IsTrue((metaData[0].MetaEventType.Equals("TimeSignature"))
        //    & (metaData[1].MetaEventType.Equals("KeySignature"))
        //    & (metaData[2].MetaEventType.Equals("Tempo"))
        //    );
        var META_DATA = new Dictionary<string, List<float>>()
        {          
            {"timeSig", new List<float> { metaData[0].Arg2, metaData[0].Arg3 }},              // (numerator, denominator)
            {"keySig", new List<float> { metaData[1].Arg2, metaData[1].Arg3 }},               // TODO: Understand (sharpsFlats, majorMinor)
            {"tempo", new List<float> { metaData[2].Arg2 }}
        };

        // #############################################################################
        // ################################# NOTE DATA #################################
        // #############################################################################

        var noteData = midiFile.Tracks[1].MidiEvents;

        string debugSTRING = "";

        // TODO: Can utilize a .removeAt(index) and DP approach to save time
        //       but will use .remove for speed right now
        Dictionary<int, (float, float)> activeNotes = new Dictionary<int, (float, float)>();
        List<Note> playHistory = new List<Note>();

        foreach (var midiEvent in noteData)
        {
            if ((midiEvent.MidiEventType == MidiEventType.NoteOn) || (midiEvent.MidiEventType == MidiEventType.NoteOff))
            {
                //var channel = midiEvent.Channel;
                int note = midiEvent.Note;
                float volume = midiEvent.Velocity;
                float timeAction = (float) midiEvent.Time;

                if (midiEvent.MidiEventType == MidiEventType.NoteOn)
                {
                    // By definition, if we're in this branch, we're encountering/pressing a note for the first time
                    activeNotes.Add(note, (timeAction, volume));
                }
                else
                {
                    // Assumption: If we're turning off a note, it must have aleady been on.
                    //             Turns out that this isn't always true, we need data sanitation.
                    if (!activeNotes.ContainsKey(note)) { continue; }
                    float initTimeBeat = activeNotes[note].Item1 / ticksPerQuarterNote;
                    float noteDuration = (timeAction / ticksPerQuarterNote) - initTimeBeat;
                    float initVolume = activeNotes[note].Item2;
                    activeNotes.Remove(note);
                    Note completedNote = new Note((NotePitch) note, initTimeBeat, noteDuration, initVolume);
                    playHistory.Add(completedNote);

                    if (debug)
                    {
                        debugSTRING += $"Note: {completedNote.Pitch},\tTime: {completedNote.Time},\tDuration {completedNote.Duration},\tVolume: {completedNote.Volume}\n";
                    }
                }
            }
        }

        if (debug)
        {
            Debug.Log(debugSTRING);
        }

        // #############################################################################
        // ###################### POPULATE INTO SHEET MUSIC CLASS ######################
        // #############################################################################
        TimeSignature ts = new TimeSignature((int) META_DATA["timeSig"][0], (int) META_DATA["timeSig"][1]);
        SheetMusic sheet = new SheetMusic(META_DATA["tempo"][0], ts, playHistory);

        return sheet;
    }

    void Start()
    {
        SheetMusic sheet = getDetails("/twinkle.mid", true);
    }
}
