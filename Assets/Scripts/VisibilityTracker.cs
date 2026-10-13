using UnityEngine;

public class VisibilityTracker : MonoBehaviour
{
    [SerializeField] private Camera observerCamera;
    [SerializeField] private MovementReplay replay;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private MovementRecorder recorder;

    private bool wasReplaying;
    private bool visibilityRecorded;

    private void Update()
    {
        bool replayJustStarted =
            replay.IsReplaying && !wasReplaying;

        if (replayJustStarted)
        {
            visibilityRecorded = false;
        }

        if (replay.IsReplaying && !visibilityRecorded)
        {
            CheckVisibility();
        }

        wasReplaying = replay.IsReplaying;
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
               visibilityRecorded = true;

               float visibleAtSeconds =
                   replay.CurrentReplayTime;

               float visibleAtMilliseconds =
                   visibleAtSeconds * 1000f;

               Debug.Log(
                   "Ghost first visible at: " +
                   visibleAtMilliseconds.ToString("F0") +
                   " ms"
               );

               ReportExposureBeforeShot(visibleAtSeconds);
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
   private void ReportExposureBeforeShot(
       float visibleAtSeconds
   )

   {
       foreach (float shotTime in recorder.ShotTimes)
       {
           if (shotTime >= visibleAtSeconds)
           {
               float exposureTime =
                   shotTime - visibleAtSeconds;

               Debug.Log(
                   "Exposed before first shot: " +
                   (exposureTime * 1000f).ToString("F0") +
                   " ms"
               );

               return;
           }
       }

       Debug.Log(
           "No shot was recorded after first visibility."
       );
   }

}