using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SheetMusic;
using static NotePitch;
using static Metrics;



//public class KeyboardInput: MonoBehaviour
//{

//    public float Tempo { get; set;  }

//    public float GroundTime { get; set; }

//    public List<Note> NotesPlayed { get; } = new List<Note>();

//    public float StartTime { get; set; }

//    public bool gameStarted { get; set; }

//    public SheetMusic Music { get; set; }

//    //public float lowerBeatBoundToConsider { get; set; }

//    //public float upperBeatBoundToConsider { get; set; }

//    private static readonly IReadOnlyDictionary<KeyCode, NotePitch> keyCodeToPitch = new Dictionary<KeyCode, NotePitch>() {
//        {KeyCode.A, B4},
//        {KeyCode.S, C5},
//        {KeyCode.D, D5},
//        {KeyCode.F, E5},
//        {KeyCode.G, F5},
//        {KeyCode.H, G5},
//        {KeyCode.J, A5},
//        {KeyCode.K, B5},
//        {KeyCode.L, C6},
//        {KeyCode.R, Eb5},
//        {KeyCode.E, Db5},
//        {KeyCode.Y, Gb5},
//        {KeyCode.U, Ab5},
//        {KeyCode.I, Bb5},
//        {KeyCode.P, B5},
//    };

//    private static float SecondsToBeats(float sec, float tempo)
//    {
//        return sec * (tempo / 60);
//    }

//    private readonly Dictionary<NotePitch, float> keysHeld = new();

//    //private static float DEFAULT_LAPTOP_KEYBOARD_VOLUME = 20;

//    //private static float ALLOWED_EPSILON = 1 / 8;

//    //public float DEFAULT_RH_RANGE_EXPECTATION;     // This is the number of notes from C4 to C5

//    //public List<NotePitch> POSSIBLE_RHS;

//    //public float lastRecordedTime;

//    ////public bool expectedActivated(NotePitch candidate, float currentTimeInBeats)
//    //{
//    //    //(indexOfLB, indexOfUB) = Music.Notes[indexOfLB];
//    //    return true;
//    //}

//    //private List<int> ExpectedActiveNotes = new List<int>();

//    ///// <summary>
//    ///// The number of elements in the list corresponds to an assumption of how many notes we expect the right-hand to play.
//    ///// </summary>
//    ///// <param name="currentTimeInBeats">This is the currentTime in beats; used to calculate which notes should be played w.r.t to the piece</param>
//    ///// <returns></returns>
//    //public void retrieveExpectedActiveNotes(float currentTimeInBeats)
//    //{
//    //    //// Base case, when we know we're starting off
//    //    //if (currentTimeInBeats >= 0 && currentTimeInBeats <= ALLOWED_EPSILON)
//    //    //{
//    //    //    // Right when we start off the update loop once the game has started,
//    //    //    // we know that the only expected notes are all the ones where the time (in beats) = 0
//    //    //    // according to the TRUTH Music object.
//    //    //    foreach (Note note in Music.Notes)
//    //    //    {
//    //    //        actualNotesWeExpect.Add(note.Pitch);
//    //    //        if (note.Time > 0) { break; }
//    //    //    }
//    //    //    // Generates the one-hot vector
//    //    //    foreach (NotePitch note in POSSIBLE_RHS)
//    //    //    {
//    //    //        expectedNotes.Add((actualNotesWeExpect.Contains(note)) ? 1 : 0);
//    //    //    }
//    //    //}
//    //    //else
//    //    //{
//    //    var trunc_currentTimeInBeats = (float) System.Math.Ceiling(currentTimeInBeats / 0.1f) * 0.1f;
//    //    //if ((currentTimeInBeats >= currentTimeInBeats - ALLOWED_EPSILON) || (currentTimeInBeats <= currentTimeInBeats + ALLOWED_EPSILON))
//    //    //{
//    //    if (currentTimeInBeats == System.Math.Truncate(trunc_currentTimeInBeats)) {
//    //        ExpectedActiveNotes.Clear();
//    //        List<NotePitch> actualNotesWeExpect = new List<NotePitch>();
//    //        foreach (Note note in Music.Notes)
//    //        {
//    //            // If a note in the Music is w/i some eps of current time, then we expect it to be played (naturally looked forward)
//    //            // ASSUMPTION: Must have a small EPSILON
//    //            if (note.Time == currentTimeInBeats)
//    //            {
//    //                actualNotesWeExpect.Add(note.Pitch);
//    //            }
//    //            else
//    //            {
//    //                break;
//    //            }
//    //        }
//    //        // Generates the one-hot vector
//    //        foreach (NotePitch _note in POSSIBLE_RHS)
//    //        {
//    //            ExpectedActiveNotes.Add((actualNotesWeExpect.Contains(_note)) ? 1 : 0);
//    //        }
//    //        //}
//    //    }
//    //    // This is the hard case

//    //}

//    void Update()
//    {
//        ////float beatConversion = SecondsToBeats(Time.time, Tempo);
//        //float beatConversion = 2.9f;
//        //retrieveExpectedActiveNotes(beatConversion);
//        //string toP = "";
//        //foreach (int elem in ExpectedActiveNotes)
//        //{
//        //    toP += $"{elem} ";
//        //}
//        //Debug.Log($"{toP}" + "\n" + "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
//        //// ~~~~~~~~~~~~~~~~ FOR LAPTOP KEYBOARD INPUT ~~~~~~~~~~~~~~~~ 
//        //// If there's no space, do not start recording laptop keyboard input
//        //upperBeatBoundToConsider += SecondsToBeats(Time.time - lastRecordedTime, Tempo);
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            gameStarted = true;
//        }

//        if (gameStarted)
//        {
//            foreach (KeyCode keyOfInterest in NoteKeyCodes)
//            {
//                if (Input.GetKeyDown(keyOfInterest))
//                {
//                    StartTime = Time.time;
//                }

//                if (Input.GetKeyUp(keyOfInterest))
//                {
//                    Debug.Log("Amount of time Pressed for key: " + keyOfInterest);
//                    float noteDuration = SecondsToBeats(Time.time - StartTime, Tempo);
//                    NotePitch notePitch = KeyCodeToPitch[keyOfInterest];
//                    NotesPlayed.Add(new Note(notePitch, SecondsToBeats(StartTime, Tempo), noteDuration, DEFAULT_LAPTOP_KEYBOARD_VOLUME));
//                }
//            }

//            foreach (KeyCode key in )
//        }

//        // ~~~~~~~~~~~~~~~~ FOR REAL-LIFE KEYBOARD INPUT ~~~~~~~~~~~~~~~~ 
//    }

//}

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