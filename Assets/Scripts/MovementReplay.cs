using UnityEngine;

public class MovementReplay : MonoBehaviour
{
    [SerializeField] private MovementRecorder recorder;

    private Renderer ghostRenderer;

    private bool isReplaying;
    private float replayStartedAt;
    private int frameIndex;

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

        frameIndex = 0;
        replayStartedAt = Time.time;
        isReplaying = true;
        ghostRenderer.enabled = true;

        Debug.Log("Movement replay started.");
        return true;
    }

    private void ReplayCurrentFrame()
    {
        float replayTime = Time.time - replayStartedAt;
        int lastFrameIndex = recorder.Frames.Count - 1;

        while (
            frameIndex < lastFrameIndex - 1 &&
            recorder.Frames[frameIndex + 1].time <= replayTime
        )
        {
            frameIndex++;
        }

        if (replayTime >= recorder.Frames[lastFrameIndex].time)
        {
            MovementFrame lastFrame =
                recorder.Frames[lastFrameIndex];

            transform.SetPositionAndRotation(
                lastFrame.position,
                lastFrame.rotation
            );

            isReplaying = false;
            Debug.Log("Movement replay finished.");
            return;
        }

        MovementFrame currentFrame =
            recorder.Frames[frameIndex];

        MovementFrame nextFrame =
            recorder.Frames[frameIndex + 1];

        float interpolationAmount = Mathf.InverseLerp(
            currentFrame.time,
            nextFrame.time,
            replayTime
        );

        Vector3 smoothPosition = Vector3.Lerp(
            currentFrame.position,
            nextFrame.position,
            interpolationAmount
        );

        Quaternion smoothRotation = Quaternion.Slerp(
            currentFrame.rotation,
            nextFrame.rotation,
            interpolationAmount
        );

        transform.SetPositionAndRotation(
            smoothPosition,
            smoothRotation
        );
    }
}