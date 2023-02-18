using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffNote : MonoBehaviour
{
    public static StaffNote Create(Staff staff, SheetMusic.Note note)
    {
        return null;
    }

    public enum NoteType
    {
        Sixteenth,
        Eighth,
        Quarter,
        Half,
        Whole,
    }

    private SheetMusic.Note note;

    private void Start()
    {
        
    }
}
