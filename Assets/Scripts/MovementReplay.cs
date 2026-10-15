using System.Collections.Generic;
using UnityEngine;

public class MovementReplay : MonoBehaviour
{
    [SerializeField] private MovementRecorder recorder;

    private Renderer ghostRenderer;
    private readonly List<float> frameTimes = new List<float>();

    private bool isReplaying;
    private float replayStartedAt;

    private void Awake()
    {
        ghostRenderer = GetComponent<Renderer>();
        ghostRenderer.enabled = false;
    }

    public bool IsReplaying => isReplaying;

    public float CurrentReplayTime =>
        isReplaying ? Time.time - replayStartedAt : 0f;

    private void Update()
    {
        if (isReplaying)
        {
            ReplayCurrentFrame();
        }
    }

    public void HideGhost()
    {
        isReplaying = false;
        ghostRenderer.enabled = false;
    }

    public bool HasRecording =>
        recorder.Frames.Count > 0;

    public bool StartReplay()
    {
        if (
            isReplaying ||
            recorder.IsRecording ||
            !HasRecording
        )
        {
            return false;
        }

        CacheFrameTimes();
        replayStartedAt = Time.time;
        isReplaying = true;
        ghostRenderer.enabled = true;

        Debug.Log("Movement replay started.");
        return true;
    }

    private void ReplayCurrentFrame()
    {
        float replayTime = Time.time - replayStartedAt;

        bool foundSegment = ReplayTimelineCalculator.TryCalculate(
            frameTimes,
            replayTime,
            out ReplaySegment segment
        );

        if (!foundSegment)
        {
            HideGhost();
            return;
        }

        if (segment.IsFinished)
        {
            MovementFrame lastFrame =
                recorder.Frames[segment.FromIndex];

            transform.SetPositionAndRotation(
                lastFrame.position,
                lastFrame.rotation
            );

            isReplaying = false;
            Debug.Log("Movement replay finished.");
            return;
        }

        MovementFrame currentFrame =
            recorder.Frames[segment.FromIndex];

        MovementFrame nextFrame =
            recorder.Frames[segment.ToIndex];

        Vector3 smoothPosition = Vector3.Lerp(
            currentFrame.position,
            nextFrame.position,
            segment.InterpolationAmount
        );

        Quaternion smoothRotation = Quaternion.Slerp(
            currentFrame.rotation,
            nextFrame.rotation,
            segment.InterpolationAmount
        );

        transform.SetPositionAndRotation(
            smoothPosition,
            smoothRotation
        );
    }

    private void CacheFrameTimes()
    {
        frameTimes.Clear();

        foreach (MovementFrame frame in recorder.Frames)
        {
            frameTimes.Add(frame.time);
        }
    }
}
