using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;

    private CharacterController controller;
    private bool wasMoving;
    private float stoppedAt;

    public bool IsMoving { get; private set; }
    public float CurrentHorizontalSpeed { get; private set; }

    public float StoppedDurationSeconds =>
        IsMoving ? 0f : Time.time - stoppedAt;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        ResetMovementState();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed) horizontal -= 1f;
        if (Keyboard.current.dKey.isPressed) horizontal += 1f;
        if (Keyboard.current.sKey.isPressed) vertical -= 1f;
        if (Keyboard.current.wKey.isPressed) vertical += 1f;

        Vector3 direction =
            (
                transform.right * horizontal +
                transform.forward * vertical
            ).normalized;

        bool movingNow =
            direction.sqrMagnitude > 0.001f;

        if (wasMoving && !movingNow)
        {
            stoppedAt = Time.time;
        }

        IsMoving = movingNow;
        CurrentHorizontalSpeed =
            movingNow ? moveSpeed : 0f;

        wasMoving = movingNow;

        controller.Move(
            direction * moveSpeed * Time.deltaTime
        );
    }

    public void ResetMovementState()
    {
        IsMoving = false;
        wasMoving = false;
        CurrentHorizontalSpeed = 0f;
        stoppedAt = Time.time;
    }
}