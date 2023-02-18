using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Staff : MonoBehaviour
{
    [Tooltip("The height in pixels of the staff")]
    [SerializeField]
    private float staffHeight = 60f;
    public float StaffHeight => staffHeight;

    [Tooltip("The MIDI note value of the bottom line of this staff")]
    [SerializeField]
    private NotePitch bottomNote;

    [Header("References")]
    [SerializeField]
    private RectTransform staffLines;

    [SerializeField]
    private RectTransform scroll;

    private RectTransform xform;

    // The y value of the smallest (lowest pitch) line and the largest (highest pitch) line
    private float minY, maxY;

    /// <summary>
    /// The vertical space in pixels between two whole notes.
    /// </summary>
    public float SpaceHeight => (maxY - minY) / 8f;

    private void Awake()
    {
        xform = transform as RectTransform;

        // Set the height of the staff
        xform.sizeDelta = new Vector2(xform.sizeDelta.x, staffHeight);

        maxY = staffLines.GetChild(0).localPosition.y;
        minY = staffLines.GetChild(staffLines.childCount - 1).localPosition.y;
    }

    // Places a note as a child of `scroll` on the staff
    private void PlaceNote (SheetMusic.Note note)
    {

    }

    // Computes how many whole notes this note is away from `bottomNote`
    // Negative values indicate this note is that many notes lower than `bottomNote`
    //
    // This assumes that all accidental notes will be flatted--i.e. note D# will be represented as Eb, not D#
    // Note that with this scheme, notes with offset less than or equal to -2 or greater than or equal to 10
    // will require ledger lines.
    private int ComputeWholeNoteOffset(NotePitch note)
    {
        Assert.IsTrue(note != NotePitch.Rest);

        int offset = 0;
        int dir = note > bottomNote ? 1 : -1;

        for (NotePitch pitch = bottomNote;
            pitch != note;
            pitch += dir)
        {
            // Increment count as long as pitch isn't accidental
            if (SheetMusic.KeySignature.CMaj.HasNote(pitch))
                offset++;
        }

        return offset * dir;
    }

    // Determines the local y position of the note on the staff
    private float ComputeNoteHeight(NotePitch note) => minY + ComputeWholeNoteOffset(note) * SpaceHeight;
}
