using System.Collections;
using System.Collections.Generic;
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

    public struct Note
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

    //public static SheetMusic FromMidi(string filename)
    //{
    //    TimeSignature sig = new TimeSignature(4, 4);
    //    Note n1 = new Note(NotePitch.C4, 10, 4, 2);
    //    List<Note> notes = new List<Note>() { n1 };
    //    return new SheetMusic(120, sig, KeySignature.CMaj, notes);
    //}
}
