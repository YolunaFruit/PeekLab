using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SessionState
{
    Ready,
    Countdown,
    Recording,
    Review,
    Replaying,
    Results
}

public class SessionController : MonoBehaviour
{
    [SerializeField] private MovementRecorder recorder;
    [SerializeField] private MovementReplay replay;
    [SerializeField] private ViewSwitcher viewSwitcher;
    [SerializeField] private Transform playerStart;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerShoot playerShoot;
    [SerializeField] private float countdownSeconds = 3f;
    [SerializeField] private AttemptHistory attemptHistory;

    private SessionState state;
    private CharacterController characterController;

    public SessionState State => state;
    public float CountdownRemaining { get; private set; }
    public int AttemptNumber { get; private set; }

    public PeekType SelectedPeekType { get; private set; } =
        PeekType.WideSwing;

    public PeekType CurrentAttemptPeekType { get; private set; } =
        PeekType.WideSwing;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }



    private void Start()
    {
        viewSwitcher.ShowPlayerView();
        SetPlayerControls(false);
        SetState(SessionState.Ready);
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }
        HandlePeekTypeInput();
        HandleViewInput();
        HandleStateInput();
    }

    private void HandleViewInput()
    {
        bool canSwitchView =
            state != SessionState.Countdown &&
            state != SessionState.Recording &&
            state != SessionState.Replaying;

        if (
            canSwitchView &&
            Keyboard.current.vKey.wasPressedThisFrame
        )
        {
            viewSwitcher.ToggleView();
        }
    }

    private void HandleStateInput()
    {
        switch (state)
        {
            case SessionState.Ready:
                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    BeginCountdown();
                }
                break;

            case SessionState.Countdown:
                break;

            case SessionState.Recording:
                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    FinishRecording();
                }
                break;

            case SessionState.Review:
                if (Keyboard.current.pKey.wasPressedThisFrame)
                {
                    BeginReplay();
                }
                else if (
                    Keyboard.current.rKey.wasPressedThisFrame
                )
                {
                    BeginCountdown();
                }
                break;

            case SessionState.Replaying:
                if (!replay.IsReplaying)
                {
                    attemptHistory.RecordAttempt(
                        AttemptNumber,
                        CurrentAttemptPeekType
                    );
                    SetState(SessionState.Results);
                }
                break;

            case SessionState.Results:
                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    BeginCountdown();
                }
                else if (
                    Keyboard.current.pKey.wasPressedThisFrame
                )
                {
                    BeginReplay();
                }
                break;
        }
    }

    private void BeginCountdown()
    {
        CurrentAttemptPeekType = SelectedPeekType;
        replay.HideGhost();
        ResetPlayer();
        viewSwitcher.ShowPlayerView();
        SetPlayerControls(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CountdownRemaining = countdownSeconds;
        SetState(SessionState.Countdown);

        StartCoroutine(CountdownThenRecord());
    }

    private IEnumerator CountdownThenRecord()
    {
        while (CountdownRemaining > 0f)
        {
            yield return null;

            CountdownRemaining -= Time.deltaTime;
        }

        CountdownRemaining = 0f;
        AttemptNumber++;

        playerMovement.ResetMovementState();
        SetPlayerControls(true);
        recorder.StartRecording();
        SetState(SessionState.Recording);
    }

    private void FinishRecording()
    {
        recorder.StopRecording();
        SetPlayerControls(false);
        viewSwitcher.ShowOpponentView();
        SetState(SessionState.Review);
    }

    private void BeginReplay()
    {
        SetPlayerControls(false);

        if (replay.StartReplay())
        {
            SetState(SessionState.Replaying);
        }
    }

    private void SetState(SessionState newState)
    {
        state = newState;

        Debug.Log("Session state: " + state);
    }

    private void ResetPlayer()
    {
        characterController.enabled = false;

        transform.SetPositionAndRotation(
            playerStart.position,
            playerStart.rotation
        );

        characterController.enabled = true;
        mouseLook.ResetLook();
    }

    private void SetPlayerControls(bool controlsEnabled)
    {
        playerMovement.enabled = controlsEnabled;
        mouseLook.enabled = controlsEnabled;
        playerShoot.enabled = controlsEnabled;

        Cursor.lockState = controlsEnabled
            ? CursorLockMode.Locked
            : CursorLockMode.None;

        Cursor.visible = !controlsEnabled;
    }

    private void HandlePeekTypeInput()
    {
        bool canSelect =
            state == SessionState.Ready ||
            state == SessionState.Review ||
            state == SessionState.Results;

        if (!canSelect)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SelectedPeekType = PeekType.WideSwing;
        }
        else if (
            Keyboard.current.digit2Key.wasPressedThisFrame
        )
        {
            SelectedPeekType = PeekType.SlowPeek;
        }
        else if (
            Keyboard.current.digit3Key.wasPressedThisFrame
        )
        {
            SelectedPeekType = PeekType.JigglePeek;
        }
        else if (
            Keyboard.current.digit4Key.wasPressedThisFrame
        )
        {
            SelectedPeekType = PeekType.Custom;
        }
    }
}