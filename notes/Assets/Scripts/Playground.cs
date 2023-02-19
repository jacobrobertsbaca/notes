using UnityEngine;
using System.Collections;
using static SheetMusic;

public class Playground
{
	// Use this for initialization
	void Start()
	{
        SheetMusic sheet = SheetMusic.FromMIDI("twinkle-RH.mid", true);
	}
}

