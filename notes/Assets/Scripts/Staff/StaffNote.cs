using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        staffNote.Note = note;
        staffNote.offset = offset;
        return staffNote;
    }

    [Header("Errors")]
    [SerializeField] private Color errorColor;
    [SerializeField] private Color successColor;
    [SerializeField] private float errorMax;
    [SerializeField] private float successMax;

    [Header("Prefabs")]
    [SerializeField] private GameObject sixteenthPrefab;
    [SerializeField] private GameObject eighthPrefab;
    [SerializeField] private GameObject quarterPrefab;
    [SerializeField] private GameObject halfPrefab;
    [SerializeField] private GameObject wholePrefab;
    [SerializeField] private GameObject ledgerLinePrefab;

    [Header("References")]
    [SerializeField] private RectTransform noteRoot;
    [SerializeField] private RectTransform ledgerLinesRoot;

    public SheetMusic.Note Note { get; private set; }
    private Staff staff;
    private int offset;
    Dictionary<float, GameObject> notePrefabs;

    private float errorAvg = 0;
    private int errorSamples = 0;
    private Graphic[] graphics; 

    public float getError() {
        return this.errorAvg;
    }

    private void Awake()
    {
        notePrefabs = new()
        {
            {0.25f, sixteenthPrefab },
            {0.5f, eighthPrefab },
            {1f, quarterPrefab },
            {2f, halfPrefab },
            {4f, wholePrefab }
        };
    }

    private void Start()
    {
        CreateNote();
        CreateLedgerLines();
        graphics = GetComponentsInChildren<Graphic>();
    }

    public void AddErrorSample (float error)
    {
        errorSamples++;
        errorAvg = errorAvg + (error - errorAvg) / errorSamples;

        // Calculate shape color
        Color color;
        if (errorAvg >= 0)
            color = Color.Lerp(Color.white, successColor, errorAvg / successMax);
        else color = Color.Lerp(Color.white, errorColor, errorAvg / errorMax);

        foreach (var graphic in graphics)
            graphic.color = color;
    }
         
    private void CreateNote()
    {
        // Try to determine which note to instantiate based on the note duration
        GameObject notePrefab = GetNotePrefab();

        // TODO: Handle dotted notes

        if (notePrefab == null)
        {
            Debug.LogWarning($"Failed to find a note prefab for note {Note}");
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

    private GameObject GetNotePrefab()
    {

        float? best = null;
        float bestDelta = float.PositiveInfinity;

        foreach (float key in notePrefabs.Keys)
        {
            float delta = Mathf.Abs(key - Note.Duration);
            if (delta < bestDelta)
            {
                bestDelta = delta;
                best = key;
            }
        }

        if (best is null) return null;
        return notePrefabs[best.Value];
    }

    private void CreateLedgerLines()
    {
        if (offset >= kLedgerMax)
        {
            int curOffset = offset;
            if (curOffset % 2 != 0) curOffset--;
            while (curOffset >= kLedgerMax)
            {
                CreateLedgerLine(new Vector3(0, (curOffset - offset) * staff.NoteHeight, 1));
                curOffset -= 2;
            }
        }

        if (offset <= kLedgerMin)
        {
            int curOffset = offset;
            if (curOffset % 2 != 0) curOffset++;
            while (curOffset <= kLedgerMin)
            {
                CreateLedgerLine(new Vector3(0, (curOffset - offset) * staff.NoteHeight, 1));
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
