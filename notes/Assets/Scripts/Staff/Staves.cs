using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a visual collection of <see cref="Staff"/> objects.
/// </summary>
public class Staves : MonoBehaviour
{
    [Tooltip("The number of pixels appearing between two beats")]
    [SerializeField]
    private float beatDistance;
    public float BeatDistance => beatDistance;

    [Tooltip("The number of pixels on the left side of the staff reserved for the clef")]
    [SerializeField]
    private float clefRegion;
    public float ClefRegion => clefRegion;

    [Tooltip("The number of pixels after the clef region at which the playhead appears")]
    [SerializeField] private float playheadPosition;
    public float PlayheadPosition => playheadPosition;

    [Header("References")]
    [SerializeField]
    private RectTransform measureMarkerPrefab;

    [SerializeField]
    private LayoutElement clefRegionElement;

    [SerializeField]
    private RectTransform scrollRoot;

    [SerializeField]
    private RectTransform playhead;

    /// <summary>
    /// The <see cref="Staff"/> that make up this object.
    /// </summary>
    [SerializeField]
    private Staff[] staves;

    private SheetMusic music;

    private void Awake()
    {
        clefRegionElement.minWidth = clefRegion;
        clefRegionElement.preferredWidth = clefRegion;
        playhead.transform.localPosition = new(playheadPosition, transform.localPosition.y, 0);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        //SheetMusic music = new SheetMusic(60,
        //    new SheetMusic.TimeSignature(4, 4),
        //    SheetMusic.KeySignature.CMaj,
        //    new List<SheetMusic.Note>()
        //    {
        //        new SheetMusic.Note(NotePitch.G5, 0, 1),
        //        new SheetMusic.Note(NotePitch.D5, 1, 1),
        //        new SheetMusic.Note(NotePitch.D5, 2, 1),
        //        new SheetMusic.Note(NotePitch.D5, 3, 1),
        //        new SheetMusic.Note(NotePitch.G4, 4, 1),
        //        new SheetMusic.Note(NotePitch.D5, 5, 1),
        //    });

        SheetMusic music = SheetMusic.FromMIDI("twinkle.mid");
        SetupStaves(music);
        Seek(0);
    }

    public void SetupStaves (SheetMusic music)
    {
        this.music = music;

        foreach (var staff in staves)
        {
            staff.SetupStaff(music);
        }

        CreateMeasureMarkers(music);
    }

    public void Seek (float beat)
    {
        scrollRoot.transform.localPosition = new Vector3(
            PlayheadPosition - beat * BeatDistance,
            scrollRoot.transform.localPosition.y,
            0);
        foreach (var staff in staves)
        {
            staff.Seek(beat);
        }
    }
     
    private void CreateMeasureMarkers (SheetMusic music)
    {
        float songLength = music.Length;
        float beatsPerMeasure = music.Time.BeatsPerMeasure * (4f / music.Time.TotalBeatsPerMeasure);

        for (float beat = beatsPerMeasure;
            beat < songLength + beatsPerMeasure;
            beat += beatsPerMeasure)
        {
            var measureMarker = Instantiate(measureMarkerPrefab, scrollRoot);
            measureMarker.transform.localPosition = new Vector3(beatDistance * (beat - 0.5f), measureMarker.transform.localPosition.y, 0);
        }
    }

    private string seekText = "";
    private void OnGUI()
    {
        seekText = GUI.TextField(new Rect(10, 10, 200, 20), seekText, 25);
        if (GUI.Button(new Rect(10, 40, 200, 20), "Seek"))
        {
            if (float.TryParse(seekText, out float beat))
                Seek(beat);
        }
    }
}
