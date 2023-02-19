using System.Linq;
using System.Collections.Generic;
using Unity; 
using static SheetMusic; 

public static class Metrics
{
    private static float GetAccuracyScore(KeyboardInput keyInput, SheetMusic music, float beat)
    {
        // Can't gain or lose anything from playing outside of music bounds
        if (beat < 0 || beat > music.Length) return 0;

        var expected = music.GetNotesAt(beat).ToDictionary(n => n.Pitch);
        var score = 0f;

        foreach (var currentNote in keyInput.CurrentNotes())
        {
            if (expected.ContainsKey(currentNote.Pitch))
            {
                // We are playing a note that we should be playing
                score++;
                expected.Remove(currentNote.Pitch);
            } else
            {
                // We are playing a note that we shouldn't be playing
                score--;
            }
        }

        foreach (var expectedNote in expected.Keys)
        {
            // We failed to play a note! Bad!
            score--;
        }

        return score;
    }
}
