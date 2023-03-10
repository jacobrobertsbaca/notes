using DG.Tweening;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private Client localClient;
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
    [SerializeField] private CanvasGroup[] cgs;
    [SerializeField] private ResultsOverlay overlay;
    [SerializeField] private WinnerText winnerText;

    [Header("Audio")]
    [SerializeField] private AudioSource metronomeLow;
    [SerializeField] private AudioSource metronomeHigh;

    private Sequence cgSequence;

    private void Awake()
    {
        NetworkClient.RegisterHandler<StageEvent>(OnStageChanged);

        // Hide the staff initially
        staves.SetVisibility(0, 0);
        staves.SetStaffVisibility(0, 0);

        // Hide game at first
        SetVisibility(false, false);
    }

    private IEnumerator Start()
    {
        // Give staff a moment to align its transforms and recalculate layout
        yield return new WaitForSeconds(0.1f);

        // Load the music
        music = SheetMusic.FromMIDI("twinkle-V2.mid").FilterNotes(NotePitch.C4, true);
        musicLength = music.Length;
        staves.SetupStaves(music);

        // Wait for one second and then fade in game
        yield return new WaitForSeconds(1f);
        SetVisibility(true);
    }

    private void Update()
    {
        if (stage == GameStage.Playing)
        {
            float beatDelta = Time.deltaTime * music.Tempo / 60f;
            beatCounter += beatDelta;
            staves.Seek(beatCounter);

            // Update input
            input.UpdateInput(beatCounter);

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
            float error = Metrics.GetAccuracyScore(input, music, beatCounter);
            if (error > 0) localClient.lane.Plane.AddVelocity(velocityMultiplier * error);

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
        if (isLocal) localClient = client;
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
                input.BeginRecording(music.Tempo);
                input.UpdateInput(-kMeasuresBefore);
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
                staves.SetVisibility(0f);
                CameraFollow.Instance.Target = winner.lane.Plane.transform;
                winnerText.SetUsername(winner.PlayerName);
                winnerText.SetVisibility(true);

                Sequence s = DOTween.Sequence();
                s.AppendInterval(6f);
                s.AppendCallback(() => winnerText.SetVisibility(false));
                s.AppendInterval(2f);
                s.AppendCallback(() => {
                    overlay.userText.text = localClient.PlayerName;
                    overlay.accuracyText.text = "Accuracy: " + ((int) Random.Range(40, 81)).ToString();
                    var statsDict = staves.Staffs[0].stats();
                    overlay.streakText.text = $"Best Streak: {statsDict["bestStreak"]}";
                    overlay.performanceText.text = statsDict["betterThanLastTime"];
                    overlay.SetVisibility(true);
                });

                // IN PROGRESS: Generate some offline metrics once we have access to `input` (all the Notes after a run)
                // input.Notes

                break;
        }
    }

    public void DestroyGame()
    {
        Destroy(gameObject);
    }

    public void SetVisibility (bool visible, bool animate = true)
    {
        cgSequence.Kill();
        float alpha = visible ? 1 : 0;
        if (animate)
        {
            Sequence s = DOTween.Sequence();
            foreach (var cg in cgs) s.Insert(0, cg.DOFade(alpha, 0.7f));
        } else
        {
            foreach (var cg in cgs) cg.alpha = alpha;
        }
    }
}
