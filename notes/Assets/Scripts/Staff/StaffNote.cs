using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffNote : MonoBehaviour
{
    private const string kStaffNoteResource = "Staff/StaffNote";
    private const int kLedgerMax = 10;
    private const int kLedgerMin = -2;
    private const int kInvertStemLine = 4;

    public static StaffNote Create(Staff staff, SheetMusic.Note note, int offset)
    {
        GameObject staffNotePrefab = Resources.Load<GameObject>(kStaffNoteResource);
        GameObject staffNoteGO = Instantiate(staffNotePrefab);
        StaffNote staffNote = staffNoteGO.GetComponent<StaffNote>();

        staffNote.staff = staff;
        staffNote.note = note;
        staffNote.offset = offset;
        return staffNote;
    }

    [Header("Prefabs")]
    [SerializeField] private GameObject sixteenthPrefab;
    [SerializeField] private GameObject eigthPrefab;
    [SerializeField] private GameObject quarterPrefab;
    [SerializeField] private GameObject halfPrefab;
    [SerializeField] private GameObject wholePrefab;
    [SerializeField] private GameObject ledgerLinePrefab;

    [Header("References")]
    [SerializeField] private RectTransform noteRoot;
    [SerializeField] private RectTransform ledgerLinesRoot;

    private Staff staff;
    private int offset;
    private SheetMusic.Note note;

    private void Start()
    {
        CreateNote();
        CreateLedgerLines();
    }

    private void CreateNote()
    {
        GameObject notePrefab = null;

        // Try to determine which note to instantiate based on the note duration
        if (Mathf.Approximately(note.Duration, 0.25f))
            notePrefab = sixteenthPrefab;
        else if (Mathf.Approximately(note.Duration, 0.5f))
            notePrefab = eigthPrefab;
        else if (Mathf.Approximately(note.Duration, 1f))
            notePrefab = quarterPrefab;
        else if (Mathf.Approximately(note.Duration, 2f))
            notePrefab = halfPrefab;
        else if (Mathf.Approximately(note.Duration, 4f))
            notePrefab = wholePrefab;

        // TODO: Handle dotted notes

        if (notePrefab == null)
        {
            Debug.LogWarning($"Failed to find a note prefab for note {note}");
            return;
        }

        GameObject noteObject = Instantiate(notePrefab, noteRoot);
        noteObject.transform.localPosition = Vector3.zero;
        noteObject.transform.localScale *= staff.ScaleFactor;

        if (offset >= kInvertStemLine)
        {
            // Notes above this line will be inverted according to the stem rule
            // https://www.musicreadingsavant.com/the-stem-rule-how-to-know-what-direction-the-stems-should-go/
            noteObject.transform.localScale *= -1;
        }
    }

    private void CreateLedgerLines()
    {
        if (offset >= kLedgerMax)
        {
            int curOffset = offset;
            if (curOffset % 2 != 0) curOffset--;
            while (curOffset >= kLedgerMax)
            {
                CreateLedgerLine(new Vector3(0, (curOffset - offset), 1));
                curOffset -= 2;
            }
        }

        if (offset <= kLedgerMin)
        {
            int curOffset = offset;
            if (curOffset % 2 != 0) curOffset++;
            while (curOffset <= kLedgerMin)
            {
                CreateLedgerLine(new Vector3(0, (curOffset - offset), 1));
                curOffset += 2;
            }
        }
    }

    private void CreateLedgerLine(Vector3 localPosition)
    {
        var ledgerObject = Instantiate(ledgerLinePrefab, ledgerLinesRoot);
        ledgerObject.transform.localPosition = localPosition;
        ledgerObject.transform.localScale = new Vector3(staff.ScaleFactor, 1f, 1f);
    }
}
