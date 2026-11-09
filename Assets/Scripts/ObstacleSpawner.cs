using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private PlayerController2D player;
    [SerializeField] private GameManager gameManager;

    [Header("Spawn")]
    [SerializeField] private float spawnX = 11f;
    [SerializeField] private float bottomLaneY = -2.9f;
    [SerializeField] private float middleLaneY = -1.15f;
    [SerializeField] private float topLaneY = 0.6f;
    [SerializeField] private float startInterval = 2.1f;
    [SerializeField] private float minimumInterval = 1.15f;
    [SerializeField] private float intervalDecreasePerMinute = 0.25f;

    [Header("Difficulty")]
    [SerializeField] private float obstacleSpeed = 6f;
    [SerializeField] private float speedIncreasePerMinute = 0.7f;
    [Range(0f, 1f)] [SerializeField] private float pitChance = 0.25f;

    private bool spawning;
    private float timer;
    private float elapsed;
    private int lastLane = -1;

    public float CurrentObstacleSpeed => obstacleSpeed + (elapsed / 60f) * speedIncreasePerMinute;

    private void Awake()
    {
        if (gameManager == null) gameManager = FindAnyObjectByType<GameManager>();
        if (player == null) player = FindAnyObjectByType<PlayerController2D>();
    }

    private void Update()
    {
        if (!spawning || gameManager == null || gameManager.State != GameManager.GameState.Running)
            return;

        elapsed += Time.deltaTime;
        timer += Time.deltaTime;

        float minutes = elapsed / 60f;
        float interval = Mathf.Max(minimumInterval, startInterval - minutes * intervalDecreasePerMinute);

        if (timer >= interval)
        {
            timer = 0f;
            SpawnObstacle(minutes);
        }
    }

    public void SetSpawning(bool value)
    {
        spawning = value;
        if (value) timer = 0f;
    }

    private void SpawnObstacle(float elapsedMinutes)
    {
        if (obstaclePrefab == null || player == null || gameManager == null) return;

        int lane = Random.Range(0, 3);
        if (lane == lastLane && Random.value < 0.65f)
            lane = (lane + Random.Range(1, 3)) % 3;
        lastLane = lane;

        float laneY = GetLaneY(lane);
        Obstacle.ObstacleType type = Random.value < pitChance
            ? Obstacle.ObstacleType.Pit
            : Obstacle.ObstacleType.Jumpable;

        GameObject instance = Instantiate(obstaclePrefab, new Vector3(spawnX, laneY, 0f), Quaternion.identity);
        Obstacle obstacle = instance.GetComponent<Obstacle>();
        if (obstacle == null)
        {
            Debug.LogError("У префаба препятствия должен быть Obstacle", instance);
            Destroy(instance);
            return;
        }

        float speed = obstacleSpeed + elapsedMinutes * speedIncreasePerMinute;
        obstacle.Initialize(player, gameManager, lane, laneY, spawnX, speed, type);
    }

    private float GetLaneY(int lane)
    {
        switch (lane)
        {
            case 0: return bottomLaneY;
            case 2: return topLaneY;
            default: return middleLaneY;
        }
    }
}
