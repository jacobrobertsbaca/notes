using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

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
    private RectTransform scrollRoot;

    [SerializeField]
    private LayoutElement clefRegionElement;

    private Staves staves;
    private RectTransform xform;

    private Transform maxLine => staffLines.GetChild(0);
    private Transform minLine => staffLines.GetChild(staffLines.childCount - 1);

    /// <summary>
    /// The vertical space in pixels between two whole notes.
    /// </summary>
    public float NoteHeight => (maxLine.transform.localPosition.y - minLine.transform.localPosition.y) / 8f;

    private void Awake()
    {
        staves = GetComponentInParent<Staves>();

        clefRegionElement.minWidth = staves.ClefRegion;
        clefRegionElement.preferredWidth = staves.ClefRegion;

        // Set the height of the staff
        xform = transform as RectTransform;
        xform.sizeDelta = new Vector2(xform.sizeDelta.x, staffHeight);
        LayoutRebuilder.ForceRebuildLayoutImmediate(xform);
    }

    //private IEnumerator Start()
    //{
    //    yield return new WaitForEndOfFrame();
    //    PlaceNote(new SheetMusic.Note(NotePitch.D5, 1, 1));
    //}

    public void SetupStaff (SheetMusic music)
    {
        foreach (var note in music.Notes)
        {
            PlaceNote(note);
        }
    }

    public void Seek(float beat)
    {
        scrollRoot.transform.localPosition = new Vector3(
            staves.PlayheadPosition - beat * staves.BeatDistance,
            scrollRoot.transform.localPosition.y,
            0);
    }

    // Places a note as a child of `scroll` on the staff
    private void PlaceNote (SheetMusic.Note note)
    {
        int line = ComputeNoteLine(note.Pitch);
        var staffNote = StaffNote.Create(this, note, line);
        staffNote.transform.SetParent(scrollRoot);
        staffNote.transform.localPosition = new Vector3(staves.BeatDistance * note.Time, NoteHeight * (line - 4), 0);
    }

    // If `bottomNote` is the lowest note on the staff, then this function computes which line
    // this note belongs to, assuming the staff lines are numbered like so:
    //
    //                  9
    // ---------------  8
    //                  7
    // ---------------  6
    //                  5
    // ---------------  4
    //                  3
    // ---------------  2
    //                  1 
    // ---------------  0
    //                  -1
    //
    // This assumes that all accidental notes will be flatted--i.e. note D# will be represented as Eb, not D#
    // Note that with this scheme, notes with offset less than or equal to -2 or greater than or equal to 10
    // will require ledger lines.
    private int ComputeNoteLine(NotePitch note)
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
}
