using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType { Jumpable, Pit }

    [Header("Collision")]
    [SerializeField] private float hitDistanceX = 0.65f;
    [SerializeField] private float requiredJumpHeight = 0.55f;
    [SerializeField] private float destroyBehindPlayer = 3f;

    [Header("Visual offsets relative to lane center")]
    [SerializeField] private float hurdleYOffset = -0.2f;
    [SerializeField] private float pitYOffset = -0.51f;

    private PlayerController2D player;
    private GameManager manager;
    private int laneIndex;
    private float moveSpeed;
    private ObstacleType obstacleType;
    private float playerX;
    private bool collisionResolved;

    public void Initialize(PlayerController2D playerRef, GameManager gameManager, int lane,
        float laneCenterY, float spawnX, float speed, ObstacleType type)
    {
        player = playerRef;
        manager = gameManager;
        laneIndex = lane;
        moveSpeed = speed;
        obstacleType = type;
        playerX = player != null ? player.transform.position.x : -4f;

        bool isPit = obstacleType == ObstacleType.Pit;
        float y = laneCenterY + (isPit ? pitYOffset : hurdleYOffset);
        transform.position = new Vector3(spawnX, y, 0f);
        transform.localScale = isPit ? new Vector3(1.55f, 0.18f, 1f) : new Vector3(0.75f, 0.75f, 1f);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = isPit ? new Color(0.12f, 0.12f, 0.12f) : new Color(1f, 0.48f, 0.08f);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; 
    }

    private void Update()
    {
        if (manager == null || manager.State != GameManager.GameState.Running) return;

        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (!collisionResolved && player != null && transform.position.x <= playerX + hitDistanceX)
        {
            collisionResolved = true;
            if (player.CurrentLane == laneIndex)
            {
                bool mustHit = obstacleType == ObstacleType.Pit || player.CurrentJumpHeight < requiredJumpHeight;
                if (mustHit) manager.LoseLife();
            }
        }

        if (transform.position.x < playerX - destroyBehindPlayer)
            Destroy(gameObject);
    }
}
