using MidiParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Music!
/// </summary>
public class SheetMusic
{
    public struct KeySignature
    {
        public static readonly KeySignature CMaj = new KeySignature(
            NoteName.C,
            NoteName.D,
            NoteName.E,
            NoteName.F,
            NoteName.G,
            NoteName.A,
            NoteName.B
        );

        private NoteName[] notes;

        /// <summary>
        /// The note names that make up this key signature.
        /// </summary>
        public IReadOnlyList<NoteName> Notes => notes;

        public KeySignature(params NoteName[] notes)
        {
            if (notes.Length != 7) throw new System.Exception("A key signature must have seven notes!");
            this.notes = notes;
        }

        public bool HasNote(NoteName noteName) => Notes.Contains(noteName);
        public bool HasNote(NotePitch pitch) => HasNote(pitch.Name());
    }
    private const float kBeatValueConstant = 4;

    public struct TimeSignature
    {
        /// <summary>
        /// Number of quarter notes in a measure (i.e. the numerator in the time signature).
        /// </summary>
        public int BeatsPerMeasure { get; }

        /// <summary>
        /// Number of quarter notes each beat in a measure represents (i.e. the denominator in the time signature).
        /// </summary>
        public float TotalBeatsPerMeasure { get; }

        /// <summary>
        /// The number of beats a given note will be held for.
        /// </summary>
        public float BeatValue => kBeatValueConstant / TotalBeatsPerMeasure;

        /// <summary>
        /// Constructor for `TimeSignature` struct
        /// </summary>
        /// <param name="beatsPerMeasure">Number of quarter notes in a measure (i.e. the numerator in the time signature).</param>
        /// <param name="totalBeatsPerMeasure">Number of quarter notes each beat in a measure represents (i.e. the denominator in the time signature).</param>
        public TimeSignature(int beatsPerMeasure, float totalBeatsPerMeasure)
        {
            BeatsPerMeasure = beatsPerMeasure;
            TotalBeatsPerMeasure = totalBeatsPerMeasure;
        }
    }

    public struct Note : IEquatable<Note>
    {
        /// <summary>
        /// Represents a note's pitch.
        /// If this is <see cref="NotePitch.Rest"/>, then this note represents a rest.
        /// </summary>
        public NotePitch Pitch { get; }

        /// <summary>
        /// The beat (in terms of quarter notes) at which this note begins.
        /// </summary>
        public float Time { get; }

        /// <summary>
        /// How many beats (quarter notes) this note is held for.
        /// </summary>
        public float Duration { get; }

        /// <summary>
        /// The initial volume of the note, when it was first played.
        /// </summary>
        public float Volume { get; }

        public Note(NotePitch pitch, float time, float duration, float volume)
        {
            Pitch = pitch;
            Time = time;
            Duration = duration;
            Volume = volume;
        }

        public Note(NotePitch pitch, float time, float duration)
            : this(pitch, time, duration, 1.0f) {}

        public bool Equals(Note other) => Pitch == other.Pitch
            && Mathf.Approximately(Time, other.Time)
            && Mathf.Approximately(Duration, other.Duration)
            && Mathf.Approximately(Volume, other.Volume);

        public override bool Equals(object obj) => obj is Note n && Equals(n);
        public override int GetHashCode() => (Pitch, Time, Duration, Volume).GetHashCode();
        public static bool operator ==(Note lhs, Note rhs) => lhs.Equals(rhs);
        public static bool operator !=(Note lhs, Note rhs) => !(lhs == rhs);

    }

    /// <summary>
    /// The tempo of this song in beats per minute.
    /// </summary>
    public float Tempo { get; }

    /// <summary>
    /// The time signature of this song.
    /// </summary>
    public TimeSignature Time { get; }

    /// <summary>
    /// The key signature of this song.
    /// </summary>
    public KeySignature Key { get; }

    /// <summary>
    /// The notes in this song.
    /// </summary>
    public IReadOnlyList<Note> Notes { get; }

    /// <summary>
    /// The length of the song in beats
    /// </summary>
    public float Length => Notes.Max(n => n.Time + n.Duration);

    public SheetMusic(float tempo, TimeSignature time, KeySignature key, IEnumerable<Note> notes)
    {
        Tempo = tempo;
        Time = time;
        Key = key;
        Notes = new List<Note>(notes);
    }

    // Removes all notes less than or equal to pitch
    public SheetMusic FilterNotes(NotePitch pitch, bool above)
    {
        if (above)
            return new SheetMusic(Tempo, Time, Key, Notes.Where(n => (n.Pitch == NotePitch.Rest) || (n.Pitch >= pitch)));
        return new SheetMusic(Tempo, Time, Key, Notes.Where(n => (n.Pitch == NotePitch.Rest) || (n.Pitch <= pitch)));

    }

    /// <summary>
    /// Loads a <see cref="SheetMusic"/> from a MIDI file.
    /// </summary>
    /// <param name="fileName">Must be in the "StreamingAssets" directory.</param>
    /// <param name="debug"></param>
    /// <returns>A <see cref="SheetMusic"/> file.</returns>
    public static SheetMusic FromMIDI(string fileName, bool debug = false)
    {
        var midiFile = new MidiFile(Path.Combine(Application.streamingAssetsPath, fileName));

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
                float timeAction = (float)midiEvent.Time;

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
                    Note completedNote = new Note((NotePitch)note, initTimeBeat, noteDuration, initVolume);
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
        TimeSignature ts = new TimeSignature((int)META_DATA["timeSig"][0], (int)META_DATA["timeSig"][1]);
        KeySignature ks = KeySignature.CMaj;
        SheetMusic sheet = new SheetMusic(META_DATA["tempo"][0], ts, ks, playHistory);

        return sheet;
    }

    public IReadOnlyList<Note> GetNotesAt(float beat)
    {
        return (from note in Notes
                where beat >= note.Time && beat <= note.Time + note.Duration
                select note).ToList();
    }
}
