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

public class MovementRecorder : MonoBehaviour
{
    private readonly List<MovementFrame> frames =
        new List<MovementFrame>();

    private bool isRecording;
    private float recordingStartedAt;
    public IReadOnlyList<MovementFrame> Frames => frames;
    public bool IsRecording => isRecording;

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ToggleRecording();
        }

        if (isRecording)
        {
            RecordFrame();
        }
    }

    private void ToggleRecording()
    {
        if (isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    private void StartRecording()
    {
        frames.Clear();
        recordingStartedAt = Time.time;
        isRecording = true;

        Debug.Log("Movement recording started.");
    }

    private void StopRecording()
    {
        isRecording = false;

        Debug.Log(
            "Movement recording stopped. Frames recorded: " +
            frames.Count
        );
    }

    private void RecordFrame()
    {
        float elapsedTime = Time.time - recordingStartedAt;

        MovementFrame frame = new MovementFrame(
            transform.position,
            transform.rotation,
            elapsedTime
        );

        frames.Add(frame);
    }
}