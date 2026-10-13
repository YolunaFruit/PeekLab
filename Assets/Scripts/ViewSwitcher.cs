using UnityEngine;
using UnityEngine.InputSystem;

public class ViewSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject opponentCamera;
    [SerializeField] private Renderer playerRenderer;

    private bool showingOpponentView;

    private void Start()
    {
        playerCamera.SetActive(true);
        playerRenderer.enabled = true;
        opponentCamera.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            ToggleView();
        }
    }

    private void ToggleView()
    {
        showingOpponentView = !showingOpponentView;
        playerRenderer.enabled = !showingOpponentView;
        playerCamera.SetActive(!showingOpponentView);
        opponentCamera.SetActive(showingOpponentView);

        if (showingOpponentView)
        {
            Debug.Log("Opponent view");
        }
        else
        {
            Debug.Log("Player view");
        }
    }
}