using UnityEngine;

public class RoadScroller2D : MonoBehaviour
{
    [Header("Road segments")]
    [SerializeField] private Transform segmentA;
    [SerializeField] private Transform segmentB;
    [SerializeField, Min(0.1f)] private float segmentWidth = 30f;
    [SerializeField] private float recycleAtX = -30f;

    [Header("Speed")]
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField, Min(0f)] private float fallbackSpeed = 6f;

    private bool isScrolling;
    private Vector3 initialPositionA;
    private Vector3 initialPositionB;

    private void Awake()
    {
        if (segmentA != null) initialPositionA = segmentA.position;
        if (segmentB != null) initialPositionB = segmentB.position;

        if (obstacleSpawner == null)
            obstacleSpawner = FindAnyObjectByType<ObstacleSpawner>();

        if (segmentA == null || segmentB == null)
            Debug.LogError("RoadScroller2D: назначить сегмент А и Б", this);
    }

    private void Update()
    {
        if (!isScrolling || segmentA == null || segmentB == null)
            return;

        float speed = obstacleSpawner != null
            ? obstacleSpawner.CurrentObstacleSpeed
            : fallbackSpeed;

        Vector3 movement = Vector3.left * Mathf.Max(0f, speed) * Time.deltaTime;
        segmentA.position += movement;
        segmentB.position += movement;

        RecycleIfNeeded(segmentA);
        RecycleIfNeeded(segmentB);
    }

    private void RecycleIfNeeded(Transform segment)
    {
        if (segment.position.x > recycleAtX)
            return;

        float rightmostX = Mathf.Max(segmentA.position.x, segmentB.position.x);
        Vector3 position = segment.position;
        position.x = rightmostX + segmentWidth;
        segment.position = position;
    }

    public void SetScrolling(bool value)
    {
        isScrolling = value;
    }

    public void ResetSegments()
    {
        if (segmentA != null) segmentA.position = initialPositionA;
        if (segmentB != null) segmentB.position = initialPositionB;
    }
}
