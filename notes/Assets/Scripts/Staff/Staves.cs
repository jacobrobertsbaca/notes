using DG.Tweening;
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

    [SerializeField]
    private CanvasGroup cg;

    [SerializeField]
    private CanvasGroup elementCg;

    /// <summary>
    /// The <see cref="Staff"/> that make up this object.
    /// </summary>
    [SerializeField]
    private Staff[] staves;

    private SheetMusic music;
    private Tween cgTween, elementCgTween;

    private void Awake()
    {
        clefRegionElement.minWidth = clefRegion;
        clefRegionElement.preferredWidth = clefRegion;
        playhead.transform.localPosition = new(playheadPosition, playhead.transform.localPosition.y, 0);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
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

    // Sets visibility of the entire staff
    public void SetVisibility (float alpha, float duration = 0.7f)
    {
        cgTween.Kill();
        cgTween = cg.DOFade(alpha, duration);
    }

    // Sets visibility of only the staff elements
    public void SetStaffVisibility (float alpha, float duration = 0.7f)
    {
        elementCgTween.Kill();
        elementCgTween = elementCg.DOFade(alpha, duration);
        foreach (var staff in staves)
        {
            staff.SetStaffVisibility(alpha, duration);
        }
    }

    public void SampleError(KeyboardInput input)
    {
        foreach (var staff in staves)
            staff.SampleError(input);
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
