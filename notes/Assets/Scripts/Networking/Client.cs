using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : NetworkBehaviour
{
    const float kMovementMultiplier = 20;

    private NetworkTransformBase netform;

    private Game game;
    public Lane lane { get; private set; }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        game = Game.GetOrCreateGame();
        lane = game.AddLane(this);

        netform = GetComponent<NetworkTransformBase>();
        netform.target = lane.Plane.transform;
        
        if (isLocalPlayer)
            CameraFollow.Instance.Target = lane.Plane.transform;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        game.RemoveLane(this);
    }

    public override void OnStopLocalPlayer()
    {
        base.OnStopLocalPlayer();
        game.DestroyGame();
    }

    private void LateUpdate()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                lane.Plane.transform.localPosition += Vector3.left * kMovementMultiplier * Time.deltaTime;
            else if (Input.GetKey(KeyCode.RightArrow))
                lane.Plane.transform.localPosition += Vector3.right * kMovementMultiplier * Time.deltaTime;
        }

        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                game.BeginGame();
        }
    }
}
