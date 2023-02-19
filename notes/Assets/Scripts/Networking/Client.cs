using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Client : NetworkBehaviour
{
    private readonly string[] kPlayerNames = { "RobberBarron", "PreFrosh", "TheBigFreeze", "Leland", "AxeAttack", "MTL" };
    const float kMovementMultiplier = 20;

    private NetworkTransformBase netform;

    private Game game;
    public Lane lane { get; private set; }

    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    private string playerName;
    public string PlayerName => playerName;

    public override void OnStartClient()
    {
        base.OnStartClient();
        game = Game.GetOrCreateGame();
        lane = game.AddLane(this, isLocalPlayer);

        netform = GetComponent<NetworkTransformBase>();
        netform.target = lane.Plane.transform;

        if (isLocalPlayer)
        {
            CameraFollow.Instance.Target = lane.Plane.transform;
            SetPlayerName(GetUsername());
        }

        OnPlayerNameChanged(null, playerName);
    }

    [Command]
    private void SetPlayerName(string name)
    {
        playerName = name;
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

    private string GetUsername()
    {
        Client[] clients = FindObjectsOfType<Client>();
        string username = kPlayerNames[Random.Range(0, kPlayerNames.Length)];
        while (clients.Any(c => c.PlayerName == username))
        {
            username = kPlayerNames[Random.Range(0, kPlayerNames.Length)];
        }
        return username;
    }

    private void LateUpdate()
    {
        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                game.BeginGame();
        }
    }

    private void OnPlayerNameChanged(string oldName, string newName)
    {
        if (lane != null) lane.Plane.SetName(newName);
    }
}
