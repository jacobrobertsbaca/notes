using System.Linq;
using System.Collections.Generic;
using static SheetMusic; 

public static class Metrics
{
    private static float GetAccuracyScore(KeyboardInput keyInput, PianoInput pianoInput, SheetMusic music, float beat)
    {
        var expected = music.GetNotesAt(beat).ToDictionary(n => n.Pitch);
        var score = 0f;

        List<Note> allCandidates = keyInput.CurrentNotes().Union(pianoInput.CurrentNotes()).ToList();

        foreach (var currentNote in allCandidates)
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
