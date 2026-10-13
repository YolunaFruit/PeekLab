using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct MovementFrame
{
    public Vector3 position;
    public Quaternion rotation;
    public float time;

    public MovementFrame(
        Vector3 position,
        Quaternion rotation,
        float time
    )
    {
        this.position = position;
        this.rotation = rotation;
        this.time = time;
    }
}

public struct ShotSample
{
    public float Time { get; }
    public float MovementSpeed { get; }
    public bool WasStable { get; }
    public float StoppedForMilliseconds { get; }

    public ShotSample(
        float time,
        float movementSpeed,
        bool wasStable,
        float stoppedForMilliseconds
    )
    {
        Time = time;
        MovementSpeed = movementSpeed;
        WasStable = wasStable;
        StoppedForMilliseconds =
            stoppedForMilliseconds;
    }
}

public class MovementRecorder : MonoBehaviour
{
    private readonly List<MovementFrame> frames =
        new List<MovementFrame>();

    private readonly List<ShotSample> shots =
        new List<ShotSample>();

    private bool isRecording;
    private float recordingStartedAt;

    public IReadOnlyList<MovementFrame> Frames => frames;
    public bool IsRecording => isRecording;


    public IReadOnlyList<ShotSample> Shots => shots; // shoot
    public int ShotCount => shots.Count;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public float LastRecordingDuration { get; private set; }

    public float CurrentRecordingDuration =>
        isRecording
            ? Time.time - recordingStartedAt
            : LastRecordingDuration;

    private void Update()
    {
        if (!isRecording)
        {
            return;
        }

        if (
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame
        )
        {
            RecordShot();
        }

        RecordFrame();
    }

    public void StartRecording()
    {
        if (isRecording)
        {
            return;
        }

        frames.Clear();
        shots.Clear();

        LastRecordingDuration = 0f;
        recordingStartedAt = Time.time;
        isRecording = true;

        Debug.Log("Movement recording started.");
    }

    public void StopRecording()
    {
        if (!isRecording)
        {
            return;
        }

        LastRecordingDuration =
            Time.time - recordingStartedAt;

        isRecording = false;

        Debug.Log(
            "Movement recording stopped. Frames recorded: " +
            frames.Count
        );
    }

    private void RecordFrame()
    {
        float elapsedTime =
            Time.time - recordingStartedAt;

        MovementFrame frame = new MovementFrame(
            transform.position,
            transform.rotation,
            elapsedTime
        );

        frames.Add(frame);
    }

    private void RecordShot()
    {
        float shotTime =
            Time.time - recordingStartedAt;

        bool wasStable =
            !playerMovement.IsMoving;

        float stoppedForMilliseconds =
            playerMovement.StoppedDurationSeconds * 1000f;

        ShotSample shot = new ShotSample(
            shotTime,
            playerMovement.CurrentHorizontalSpeed,
            wasStable,
            stoppedForMilliseconds
        );

        shots.Add(shot);

        Debug.Log(
            "Shot recorded at: " +
            (shotTime * 1000f).ToString("F0") +
            " ms | Stable: " +
            wasStable
        );
    }
}