using UnityEngine;

public class WorldScroller : MonoBehaviour
{
    [SerializeField] private Transform tileA;
    [SerializeField] private Transform tileB;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float tileWidth = 20f;
    [SerializeField] private GameManager gameManager;

    private void Update()
    {
        if (tileA == null || tileB == null) return;
        if (gameManager != null && gameManager.State != GameManager.GameState.Running) return;

        Vector3 movement = Vector3.left * speed * Time.deltaTime;
        tileA.position += movement;
        tileB.position += movement;

        if (tileA.position.x <= -tileWidth)
            tileA.position = new Vector3(tileB.position.x + tileWidth, tileA.position.y, tileA.position.z);
        if (tileB.position.x <= -tileWidth)
            tileB.position = new Vector3(tileA.position.x + tileWidth, tileB.position.y, tileB.position.z);
    }
}
