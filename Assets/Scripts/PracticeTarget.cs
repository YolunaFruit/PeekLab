using System.Collections;
using UnityEngine;

public class PracticeTarget : MonoBehaviour
{
    [SerializeField] private float minimumDelay = 1f;
    [SerializeField] private float maximumDelay = 3f;

    private Renderer targetRenderer;
    private Collider targetCollider;

    private float appearedAt;
    private bool isActive;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        targetCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        StartCoroutine(BeginRound());
    }

    private IEnumerator BeginRound()
    {
        isActive = false;
        targetRenderer.enabled = false;
        targetCollider.enabled = false;

        float delay = Random.Range(minimumDelay, maximumDelay);
        yield return new WaitForSeconds(delay);

        targetRenderer.enabled = true;
        targetCollider.enabled = true;

        appearedAt = Time.time;
        isActive = true;
    }

    public void RegisterHit(Vector3 hitPoint)
    {
        if (!isActive)
        {
            return;
        }

        float reactionTimeSeconds = Time.time - appearedAt;
        float reactionTimeMilliseconds = reactionTimeSeconds * 1000f;

        Debug.Log(
            "Reaction time: " +
            reactionTimeMilliseconds.ToString("F0") +
            " ms"
        );

        StartCoroutine(BeginRound());
    }
}