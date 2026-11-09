using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum RewardType { Score, ExtraLife }

    [SerializeField] private RewardType rewardType = RewardType.Score;
    [SerializeField] private int scoreValue = 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager manager = FindAnyObjectByType<GameManager>();
        if (manager != null)
        {
            if (rewardType == RewardType.Score)
                manager.AddScore(scoreValue);
            else
                manager.AddLife(1);
        }
        Destroy(gameObject);
    }
}
