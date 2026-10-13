using System.Collections.Generic;
using UnityEngine;

public struct AttemptResult
{
    public int AttemptNumber { get; }
    public PeekType Technique { get; }
    public float DurationSeconds { get; }
    public int ShotCount { get; }
    public bool HasVisibilityResult { get; }
    public float FirstVisibleMilliseconds { get; }
    public bool HasShotAfterVisibility { get; }
    public float ExposureMilliseconds { get; }

    public bool ShotWasStable { get; }
    public float ShotMovementSpeed { get; }
    public float StoppedBeforeShotMilliseconds { get; }

    public AttemptResult(
        int attemptNumber,
        PeekType technique,
        float durationSeconds,
        int shotCount,
        bool hasVisibilityResult,
        float firstVisibleMilliseconds,
        bool hasShotAfterVisibility,
        float exposureMilliseconds,
        bool shotWasStable,
        float shotMovementSpeed,
        float stoppedBeforeShotMilliseconds
    )
    {
        AttemptNumber = attemptNumber;
        Technique = technique;
        DurationSeconds = durationSeconds;
        ShotCount = shotCount;
        HasVisibilityResult = hasVisibilityResult;
        FirstVisibleMilliseconds = firstVisibleMilliseconds;
        HasShotAfterVisibility = hasShotAfterVisibility;
        ExposureMilliseconds = exposureMilliseconds;
        ShotWasStable = shotWasStable;
        ShotMovementSpeed = shotMovementSpeed;
        StoppedBeforeShotMilliseconds =
        stoppedBeforeShotMilliseconds;
    }
}

public class AttemptHistory : MonoBehaviour
{
    private readonly List<AttemptResult> attempts =
        new List<AttemptResult>();

    private MovementRecorder recorder;
    private VisibilityTracker visibilityTracker;

    public int Count => attempts.Count;
    public bool HasLatest => attempts.Count > 0;

    public AttemptResult Latest =>
        attempts[attempts.Count - 1];

    public bool HasBestExposure { get; private set; }
    public float BestExposureMilliseconds { get; private set; }

    private void Awake()
    {
        recorder = GetComponent<MovementRecorder>();
        visibilityTracker = GetComponent<VisibilityTracker>();
    }

    public void RecordAttempt(
        int attemptNumber,
        PeekType technique
    )
    {
        if (
            HasLatest &&
            Latest.AttemptNumber == attemptNumber
        )
        {
            return;
        }

        AttemptResult result = new AttemptResult(
            attemptNumber,
            technique,
            recorder.LastRecordingDuration,
            recorder.ShotCount,
            visibilityTracker.HasVisibilityResult,
            visibilityTracker.FirstVisibleMilliseconds,
            visibilityTracker.HasShotAfterVisibility,
            visibilityTracker.ExposureBeforeShotMilliseconds,
            visibilityTracker.ShotWasStable,
            visibilityTracker.ShotMovementSpeed,
            visibilityTracker.StoppedBeforeShotMilliseconds
        );

        attempts.Add(result);

        if (result.HasShotAfterVisibility)
        {
            if (
                !HasBestExposure ||
                result.ExposureMilliseconds <
                BestExposureMilliseconds
            )
            {
                HasBestExposure = true;
                BestExposureMilliseconds =
                    result.ExposureMilliseconds;
            }
        }

        Debug.Log(
            "Saved result for attempt " +
            attemptNumber
        );
    }

    public bool TryGetPreviousExposure(
        out float previousExposure
    )
    {
        for (int index = attempts.Count - 2; index >= 0; index--)
        {
            if (attempts[index].HasShotAfterVisibility)
            {
                previousExposure =
                    attempts[index].ExposureMilliseconds;

                return true;
            }
        }

        previousExposure = 0f;
        return false;
    }
}