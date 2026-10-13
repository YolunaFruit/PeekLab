using UnityEngine;
using UnityEngine.InputSystem;

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

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.pKey.wasPressedThisFrame &&
            !isReplaying)
        {
            StartReplay();
        }

        if (isReplaying)
        {
            ReplayCurrentFrame();
        }
    }

    private void StartReplay()
    {
        if (recorder.IsRecording)
        {
            Debug.Log("Stop recording before starting the replay.");
            return;
        }

        if (recorder.Frames.Count == 0)
        {
            Debug.Log("There is no movement recording yet.");
            return;
        }

        frameIndex = 0;
        replayStartedAt = Time.time;
        isReplaying = true;
        ghostRenderer.enabled = true;

        Debug.Log("Movement replay started.");
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