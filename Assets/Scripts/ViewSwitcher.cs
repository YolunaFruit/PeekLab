using UnityEngine;

public class ViewSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject opponentCamera;
    [SerializeField] private Renderer playerRenderer;

    private bool showingOpponentView;

    private void Start()
    {
        ShowPlayerView();
    }

    public void ShowPlayerView()
    {
        showingOpponentView = false;
        ApplyView();
    }

    public void ShowOpponentView()
    {
        showingOpponentView = true;
        ApplyView();
    }

    public void ToggleView()
    {
        showingOpponentView = !showingOpponentView;
        ApplyView();
    }

    private void ApplyView()
    {
        playerCamera.SetActive(!showingOpponentView);
        opponentCamera.SetActive(showingOpponentView);
        playerRenderer.enabled = !showingOpponentView;

        Debug.Log(
            showingOpponentView
                ? "Opponent view"
                : "Player view"
        );
    }
}