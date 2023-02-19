using UnityEngine;
using System.Collections;
using static SheetMusic;

public class Playground : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
        //Debug.Log("23213");
        //MAKE SURE THIS FILE IS NAMED PROPERLY, OR WE'LL GET A WACK ERROR MESSAGE
        SheetMusic sheet = FromMIDI("twinkle.mid", true);
        sheet = sheet.FilterNotes(NotePitch.B3, true);
        Debug.Log($"Number of notes after stripping: {sheet.Notes.Count}");
    }
}

