using TMPro;
using UnityEngine;

public class SessionHUD : MonoBehaviour
{
    [SerializeField] private SessionController sessionController;
    [SerializeField] private MovementRecorder recorder;
    [SerializeField] private AttemptHistory attemptHistory;
    [SerializeField] private TMP_Text statusText;

    private SessionState displayedState;

    private void Start()
    {
        RefreshStatus();
    }

    private void Update()
    {
        if (sessionController.State != displayedState)
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

        switch (displayedState)
        {
            case SessionState.Ready:
                statusText.text =
                    "READY\nPress R to begin";
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
                    " RECORDED\n" +
                    "P: replay | R: record again";
                break;

            case SessionState.Replaying:
                statusText.text =
                    "REPLAYING ATTEMPT " +
                    sessionController.AttemptNumber;
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
            "\nSTARTING IN " +
            secondsRemaining;
    }

    private void UpdateRecordingText()
    {
        statusText.text =
            "RECORDING ATTEMPT " +
            sessionController.AttemptNumber +
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
}