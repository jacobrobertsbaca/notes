using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsertRoads : MonoBehaviour
{
    // Vector4 playerTracks = new Vector4[];
    // int numPlayers;
    // public GameObject road;
    // public GameObject dividerLine;
    // public GameObject playerTrackContainer;
    // public GameObject roadContainer;
    // bool newPlayerJoins = true;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     //make empty game object for player 1
    //     var player1Track = new GameObject(typeof(RectTransform));
    //     player1Track.SetParent(transform);
    //     //populate that game object with the three tracks
    //     var topLine = Instantiate(dividerLine, transform.position, Quaternion.identity);
    //     topLine.transform.SetParent(transform);
    //     var player1Road = Instantiate(road, transform.position, Quaternion.identity);
    //     player1Road.transform.SetParent(transform);
    //     var bottomLine = Instantiate(dividerLine, transform.position, Quaternion.identity);
    //     bottomLine.transform.SetParent(transform);
    //     playerTracks[0] = player1Track;
    //     numPlayers = 1;
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //  if (newPlayerJoins) {

        
        
    //     playerTracks[numPlayers] = Instantiate(newPlayerTrack, transform.position,Quaternion.identity);


    //     //Set the bottom line of the road back to the last child element (so that it shows up last)
    //     newPlayerJoins = false;
    //     numPlayers += 1;

    //     //move the previous bottomLine to the bottom of the list of people
    //  }   
    // }

    // void createPlayerTrack()
    // {

    // }

    // void createRoadSegment(float location)
    // {
    //     //make an empty object
    //     var road = new GameObject(typeof(RectTransform));
    //     road.SetParent(transform);
    //     //add the road
    //     if(numPlayers == 1){
    //         var newDividerLine1 = Instantiate(dividerLine, location, Quaternion.identity);
    //         newDividerLine1.transform.SetParent(road1);
    //     }
    //     var newPlayerRoad = Instantiate(road, location, Quaternion.identity);
    //     newPlayerRoad.transform.SetParent(road1);
    //     //Create a new white line on the bottom
    //     var newDividerLine2 = Instantiate(dividerLine, location, Quaternion.identity);
    //     newDividerLine2.transform.SetParent(road1);
    //     playerRoads.append(road);
    // }
}
