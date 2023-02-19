using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static KeyboardInput;
using static SheetMusic; 

public static class Metrics
{
    private static float GetAccuracyScore(KeyboardInput input, SheetMusic music, float beat)
    {
        var expected = music.GetNotesAt(beat).ToDictionary(n => n.Pitch);
        var score = 0f;

        foreach (var currentNote in input.CurrentNotes())
        {
            if (expected.ContainsKey(currentNote.Pitch))
            {
                // We are playing a note that we should be playing
                score++;
            } else
            {
                // We are playing a note that we shouldn't be playing
                score--;
            }
        }

        return score;
    }
}
