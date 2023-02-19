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
        SheetMusic sheet = FromMIDI("twinkle-V2.mid", true);
        sheet = sheet.FilterNotes(NotePitch.C4, "below");
        Debug.Log($"Number of notes after stripping: {sheet.Notes.Count}");
    }
}

