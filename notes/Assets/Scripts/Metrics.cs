using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KeyboardInput;
using static SheetMusic; 

public class Metrics : MonoBehaviour
{
    public KeyboardInput Keyboard;
    // TODO: Implement EVENT Listeners


    static string nameOfPiece = "twinkle.mid";
    SheetMusic music = FromMIDI(nameOfPiece);
    
    private void Awake()
    {
        music = music.FilterNotes(NotePitch.B3, true);
        Keyboard.Tempo = music.Tempo;
        // KeyboardInput keyboard = new KeyboardInput(music); // tempo required for beat calculation TODO: Have game pass this in instead of the SheetMusic? 
    }

    private void Update()
    {
        Debug.Log($"Something cool {music.Notes.Count}"); 
    }
}
