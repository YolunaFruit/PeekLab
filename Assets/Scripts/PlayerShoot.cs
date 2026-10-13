using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private float range = 100f;

    private Camera playerCamera;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            PracticeTarget target =
                hit.collider.GetComponent<PracticeTarget>();

            if (target != null)
            {
                target.RegisterHit(hit.point);
            }
            else
            {
                Debug.Log("Hit environment: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("Miss");
        }
    }
}