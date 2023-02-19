using DG.Tweening;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : NetworkBehaviour
{
    private enum GameStage
    {
        Waiting,
        Counting,
        Playing,
        Finished
    }

    private struct StageEvent : NetworkMessage
    {
        public GameStage Stage;
    }

    private const string kGameResource = "Game";

    // Number of measures after music finishes before finishing game
    private const int kMeasuresAfter = 2;

    // Number of measures to play before music starts
    private const int kMeasuresBefore = 8;

    private static Game current;

    private Dictionary<Client, Lane> lanes = new();
    private Lane clientLane;
    private GameStage stage = GameStage.Waiting;
    private SheetMusic music;
    private float musicLength;
    private float beatCounter = 0;
    private float metronomeCounter = 0;
    private int metronomeTicks = 0;

    [SerializeField] private float velocityMultiplier = 5f;
    [SerializeField] private float baseVelocity = 5f;

    [Header("References")]
    [SerializeField] private Staves staves;
    [SerializeField] private Lane lanePrefab;
    [SerializeField] private RectTransform laneRoot;
    [SerializeField] private Countdown countdown;
    [SerializeField] private SpawnClouds cloudSpawner;
    [SerializeField] private KeyboardInput input;

    [Header("Audio")]
    [SerializeField] private AudioSource metronomeLow;
    [SerializeField] private AudioSource metronomeHigh;

    private void Awake()
    {
        NetworkClient.RegisterHandler<StageEvent>(OnStageChanged);

        // Hide the staff initially
        staves.SetVisibility(0, 0);
        staves.SetStaffVisibility(0, 0);
    }

    private IEnumerator Start()
    {
        // Give staff a moment to align its transforms and recalculate layout
        yield return new WaitForSeconds(0.1f);

        // Load the music
        music = SheetMusic.FromMIDI("twinkle-V2.mid").FilterNotes(NotePitch.C4, true);
        musicLength = music.Length;
        staves.SetupStaves(music);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 100), "Metron"))
        {
            metronomeLow.Play();
        }
    }

    private void Update()
    {
        if (stage == GameStage.Playing)
        {
            float beatDelta = Time.deltaTime * music.Tempo / 60f;
            beatCounter += beatDelta;
            staves.Seek(beatCounter);

            // Each time we cross over a beat boundary, we play the metronome
            metronomeCounter += beatDelta;
            if (metronomeCounter >= music.Time.BeatValue)
            {
                metronomeTicks++;
                metronomeTicks %= music.Time.BeatsPerMeasure;
                metronomeCounter -= music.Time.BeatValue;
                if (metronomeTicks == 0) metronomeHigh.Play();
                else metronomeLow.Play();
            }

            // Sample error for current notes
            staves.SampleError(input);

            // Get error and boost plane accordingly
            float error = Metrics.GetAccuracyScore(input, music, input.Beat);
            if (error > 0) clientLane.Plane.AddVelocity(velocityMultiplier * error);

            if (beatCounter > musicLength + kMeasuresAfter * music.Time.BeatValue * music.Time.BeatsPerMeasure)
            {
                NetworkServer.SendToAll(new StageEvent()
                {
                    Stage = GameStage.Finished
                });
            }
        }
    }

    public static Game GetOrCreateGame ()
    {
        if (current) return current;
        var gamePrefab = Resources.Load<GameObject>(kGameResource);
        var gameGO = Instantiate(gamePrefab, null);
        current = gameGO.GetComponent<Game>();
        return current;
    }

    public Lane AddLane(Client client, bool isLocal = false)
    {
        var lane = Instantiate(lanePrefab.gameObject, laneRoot).GetComponent<Lane>();
        lanes[client] = lane;
        if (isLocal) clientLane = lane;
        return lane;
    }

    public void RemoveLane(Client client)
    {
        if (!lanes.ContainsKey(client)) return;
        Destroy(lanes[client].gameObject);
        lanes.Remove(client);
    }

    public void BeginGame ()
    {
        NetworkServer.SendToAll(new StageEvent()
        {
            Stage = GameStage.Counting
        });
    }

    private void OnStageChanged(StageEvent newStage)
    {
        if (newStage.Stage == stage) return;
        stage = newStage.Stage;

        switch (stage)
        {
            case GameStage.Counting:
                staves.SetVisibility(1f);
                countdown.StartCountdown(() => NetworkServer.SendToAll(new StageEvent()
                {
                    Stage = GameStage.Playing
                }));
                break;

            case GameStage.Playing:
                beatCounter = -kMeasuresBefore;
                input.BeginRecording(music.Tempo, -kMeasuresBefore);
                staves.SetStaffVisibility(1f);
                cloudSpawner.BeginSpawning();

                // Start moving planes
                foreach (var lane in lanes.Values)
                    lane.Plane.SetBaseVelocity(baseVelocity);

                break;

            case GameStage.Finished:
                // Stop recording
                input.StopRecording();

                // Stop moving planes
                foreach (var lane in lanes.Values)
                    lane.Plane.SetBaseVelocity(0f);

                // Who is the winner? Whoever is furthest to the right
                Client[] clients = FindObjectsOfType<Client>();
                Client winner = clients[0];
                for (int i = 1; i < clients.Length; i++)
                {
                    if (clients[i].lane.Plane.transform.position.x > winner.lane.Plane.transform.position.x)
                        winner = clients[i];
                }

                // Pan to the winner for a few seconds
                CameraFollow.Instance.Target = winner.lane.Plane.transform;
                break;
        }
    }

    public void DestroyGame()
    {
        Destroy(gameObject);
    }
}
