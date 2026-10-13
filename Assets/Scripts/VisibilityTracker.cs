using UnityEngine;

public class VisibilityTracker : MonoBehaviour
{
    [SerializeField] private Camera observerCamera;
    [SerializeField] private MovementReplay replay;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private MovementRecorder recorder;

    public bool HasVisibilityResult { get; private set; }
    public float FirstVisibleMilliseconds { get; private set; }
    public bool HasShotAfterVisibility { get; private set; }
    public float ExposureBeforeShotMilliseconds { get; private set; }

    private bool wasReplaying;
    private bool visibilityRecorded;

    public bool ShotWasStable { get; private set; }
    public float ShotMovementSpeed { get; private set; }

    public float StoppedBeforeShotMilliseconds
    {
        get;
        private set;
    }

    private void Update()
    {
        bool replayJustStarted =
            replay.IsReplaying && !wasReplaying;

        if (replayJustStarted)
        {
            ResetResults();
        }

        if (replay.IsReplaying && !visibilityRecorded)
        {
            CheckVisibility();
        }

        wasReplaying = replay.IsReplaying;
    }

    private void ResetResults()
    {
        visibilityRecorded = false;
        HasVisibilityResult = false;
        FirstVisibleMilliseconds = 0f;
        HasShotAfterVisibility = false;
        ExposureBeforeShotMilliseconds = 0f;
        ShotWasStable = false;
        ShotMovementSpeed = 0f;
        StoppedBeforeShotMilliseconds = 0f;
    }

    private void CheckVisibility()
    {
        Vector3 observerPosition =
            observerCamera.transform.position;

        Vector3 ghostCenter =
            replay.transform.position;

        Vector3 directionToGhost =
            ghostCenter - observerPosition;

        Vector3 sideDirection = Vector3.Cross(
            Vector3.up,
            directionToGhost
        ).normalized;

        Vector3 shoulderOffset = sideDirection * 0.45f;

        Vector3[] samplePoints =
        {
            ghostCenter + Vector3.up * 0.75f,
            ghostCenter,
            ghostCenter + Vector3.up * 0.25f + shoulderOffset,
            ghostCenter + Vector3.up * 0.25f - shoulderOffset
        };

        foreach (Vector3 point in samplePoints)
        {
            if (IsPointVisible(observerPosition, point))
            {
                RecordFirstVisibility();
                return;
            }
        }
    }

    private bool IsPointVisible(
        Vector3 observerPosition,
        Vector3 point
    )
    {
        Vector3 viewportPoint =
            observerCamera.WorldToViewportPoint(point);

        bool insideCameraView =
            viewportPoint.z > 0f &&
            viewportPoint.x >= 0f &&
            viewportPoint.x <= 1f &&
            viewportPoint.y >= 0f &&
            viewportPoint.y <= 1f;

        bool blockedByObstacle = Physics.Linecast(
            observerPosition,
            point,
            obstacleLayers
        );

        bool visible =
            insideCameraView && !blockedByObstacle;

        Debug.DrawLine(
            observerPosition,
            point,
            visible ? Color.green : Color.red
        );

        return visible;
    }

    private void RecordFirstVisibility()
    {
        visibilityRecorded = true;
        HasVisibilityResult = true;

        float visibleAtSeconds =
            replay.CurrentReplayTime;

        FirstVisibleMilliseconds =
            visibleAtSeconds * 1000f;

        Debug.Log(
            "Ghost first visible at: " +
            FirstVisibleMilliseconds.ToString("F0") +
            " ms"
        );

        RecordExposureBeforeShot(visibleAtSeconds);
    }

    private void RecordExposureBeforeShot(
        float visibleAtSeconds
    )

    {
        foreach (ShotSample shot in recorder.Shots)
        {
            if (shot.Time >= visibleAtSeconds)
            {
                HasShotAfterVisibility = true;

                ExposureBeforeShotMilliseconds =
                    (shot.Time - visibleAtSeconds) * 1000f;

                ShotWasStable = shot.WasStable;
                ShotMovementSpeed = shot.MovementSpeed;

                StoppedBeforeShotMilliseconds =
                    shot.StoppedForMilliseconds;

                Debug.Log(
                    "Exposed before first shot: " +
                    ExposureBeforeShotMilliseconds.ToString("F0") +
                    " ms | Stable: " +
                    ShotWasStable
                );

                return;
            }
        }

        HasShotAfterVisibility = false;

        Debug.Log(
            "No shot was recorded after first visibility."
        );
    }
}




