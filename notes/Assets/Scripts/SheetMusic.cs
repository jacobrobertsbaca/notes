using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Music!
/// </summary>
public class SheetMusic
{
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
        public float BeatValue => 4 / TotalBeatsPerMeasure;

        public TimeSignature(int beatsPerMeasure, float totalBeatsPerMeasure)
        {
            BeatsPerMeasure = beatsPerMeasure;
            TotalBeatsPerMeasure = totalBeatsPerMeasure;
        }
    }

    public struct Note
    {
        /// <summary>
        /// Represents a note's pitch. 0 corresponds to note C-1 and 127 corresponds to note G9.
        /// If this is negative, then this note represents a rest.
        /// </summary>
        public int Pitch { get; }

        /// <summary>
        /// The beat (in terms of quarter notes) at which this note begins.
        /// </summary>
        public float Time { get; }

        /// <summary>
        /// How many beats (quarter notes) this note is held for.
        /// </summary>
        public float Duration { get; }

        public Note(int pitch, float time, float duration)
        {
            Pitch = pitch;
            Time = time;
            Duration = duration;
        }
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
    /// The notes in this song.
    /// </summary>
    public IReadOnlyList<Note> Notes { get; }

    public SheetMusic(float tempo, TimeSignature time, IEnumerable<Note> notes)
    {
        Tempo = tempo;
        Time = time;
        Notes = new List<Note>(notes);
    }

    public static SheetMusic FromMidi(string filename)
    {
        TimeSignature sig = new TimeSignature(4, 4);
        Note n1 = new Note(60, 10, 4);
        List<Note> notes = new List<Note>() { n1 };
        return new SheetMusic(120, sig, notes);
    }
}
