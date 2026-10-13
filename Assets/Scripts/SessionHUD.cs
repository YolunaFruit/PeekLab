using TMPro;
using UnityEngine;

public class SessionHUD : MonoBehaviour
{
    [SerializeField] private SessionController sessionController;
    [SerializeField] private MovementRecorder recorder;
    [SerializeField] private AttemptHistory attemptHistory;
    [SerializeField] private TMP_Text statusText;

    private SessionState displayedState;
    private PeekType displayedSelection;

    private void Start()
    {
        RefreshStatus();
    }

    private void Update()
    {
        if (
            sessionController.State != displayedState ||
            sessionController.SelectedPeekType != displayedSelection
        )
        {
            RefreshStatus();
        }

        if (sessionController.State == SessionState.Countdown)
        {
            UpdateCountdownText();
        }
        else if (
            sessionController.State == SessionState.Recording
        )
        {
            UpdateRecordingText();
        }
    }

    private void RefreshStatus()
    {
        displayedState = sessionController.State;
        displayedSelection = sessionController.SelectedPeekType;

        switch (displayedState)
        {
            case SessionState.Ready:
                statusText.text =
                    "SELECT PEEK TYPE\n" +
                    "1: Wide | 2: Slow | 3: Jiggle | 4: Custom\n" +
                    "Selected: " +
                    GetPeekTypeName(
                        sessionController.SelectedPeekType
                    ) +
                    "\nPress R to begin";
                break;

            case SessionState.Countdown:
                UpdateCountdownText();
                break;

            case SessionState.Recording:
                UpdateRecordingText();
                break;

            case SessionState.Review:
                statusText.text =
                    "ATTEMPT " +
                    sessionController.AttemptNumber +
                    " — " +
                    GetPeekTypeName(
                        sessionController.CurrentAttemptPeekType
                    ) +
                    "\nP: replay | R: record again\n" +
                    "Next technique: " +
                    GetPeekTypeName(
                        sessionController.SelectedPeekType
                    );
                break;

            case SessionState.Replaying:
                statusText.text =
                    "REPLAYING ATTEMPT " +
                    sessionController.AttemptNumber +
                    "\nTechnique: " +
                    GetPeekTypeName(
                        sessionController.CurrentAttemptPeekType
                    );
                break;

            case SessionState.Results:
                statusText.text = BuildResultsText();
                break;
        }
    }

    private void UpdateCountdownText()
    {
        int secondsRemaining = Mathf.CeilToInt(
            sessionController.CountdownRemaining
        );

        statusText.text =
            "ATTEMPT " +
            (sessionController.AttemptNumber + 1) +
            "\nTechnique: " +
            GetPeekTypeName(
                sessionController.CurrentAttemptPeekType
            ) +
            "\nSTARTING IN " +
            secondsRemaining;
    }

    private void UpdateRecordingText()
    {
        statusText.text =
            "RECORDING ATTEMPT " +
            sessionController.AttemptNumber +
            "\nTechnique: " +
            GetPeekTypeName(
                sessionController.CurrentAttemptPeekType
            ) +
            "\nTime: " +
            recorder.CurrentRecordingDuration.ToString("F1") +
            " s | Shots: " +
            recorder.ShotCount +
            "\nPress R to stop";
    }

    private string BuildResultsText()
    {
        if (!attemptHistory.HasLatest)
        {
            return "RESULTS\nNo result was saved";
        }

        AttemptResult result = attemptHistory.Latest;

        string text =
            "RESULTS — ATTEMPT " +
            result.AttemptNumber +
            "\nTechnique: " +
            GetPeekTypeName(result.Technique) +
            "\nDuration: " +
            result.DurationSeconds.ToString("F1") +
            " s | Shots: " +
            result.ShotCount +
            "\n";

        if (!result.HasVisibilityResult)
        {
            return
                text +
                "You never entered the opponent's view\n" +
                "R: record again | P: replay";
        }

        text +=
            "First visible: " +
            result.FirstVisibleMilliseconds.ToString("F0") +
            " ms\n";

        if (!result.HasShotAfterVisibility)
            {
                return
                    text +
                    "No shot after becoming visible\n" +
                    "R: record again | P: replay";
        }

        text +=
            "Exposure before shot: " +
            result.ExposureMilliseconds.ToString("F0") +
            " ms\n";

        if (result.ShotWasStable) {
            text +=
                "Shot stability: STABLE\n" +
                "Stopped before shot: " +
                result.StoppedBeforeShotMilliseconds
                    .ToString("F0") +
                " ms\n";
        }
        else
        {
            text +=
                "Shot stability: MOVING\n" +
                "Movement speed: " +
                result.ShotMovementSpeed.ToString("F1") +
                " units/s\n";
        }

        if (
            attemptHistory.TryGetPreviousExposure(
                out float previousExposure
            )
        )
        {
            float difference =
                result.ExposureMilliseconds -
                previousExposure;

            if (Mathf.Abs(difference) < 1f)
            {
                text +=
                    "Same exposure as previous attempt\n";
            }
            else
            {
                string direction =
                    difference < 0f ? "less" : "more";

                text +=
                    Mathf.Abs(difference).ToString("F0") +
                    " ms " +
                    direction +
                    " exposure than previous\n";
            }
        }

        if (attemptHistory.Count > 1)
        {
            text +=
                "Best exposure: " +
                attemptHistory
                    .BestExposureMilliseconds
                    .ToString("F0") +
                " ms\n";
        }

        return
            text +
            "R: record again | P: replay";
    }

    private string GetPeekTypeName(PeekType peekType)
    {
        switch (peekType)
        {
            case PeekType.WideSwing:
                return "Wide Swing";

            case PeekType.SlowPeek:
                return "Slow Peek";

            case PeekType.JigglePeek:
                return "Jiggle Peek";

            case PeekType.Custom:
                return "Custom";

            default:
                return "Unknown";
        }
    }

}