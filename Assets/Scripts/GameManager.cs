using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState { Ready, Running, Paused, GameOver }

    [Header("Rules")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private float scorePerSecond = 1f;

    [Header("Gameplay references")]
    [SerializeField] private PlayerController2D player;
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private RoadScroller2D roadScroller;

    [Header("UI references (optional)")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    public GameState State { get; private set; } = GameState.Ready;
    public int Lives { get; private set; }
    public float Score { get; private set; }

    private void Start()
    {
        Time.timeScale = 1f;
        Lives = startingLives;
        Score = 0f;
        SetState(GameState.Ready);
        RefreshHud();
        if (player != null) player.SetCanControl(false);
        if (obstacleSpawner != null) obstacleSpawner.SetSpawning(false);
    }

    private void Update()
    {
        if (State != GameState.Running) return;
        Score += scorePerSecond * Time.deltaTime;
        RefreshHud();
    }

    public void StartGame()
    {
        if (State != GameState.Ready) return;
        SetState(GameState.Running);
        if (player != null) player.SetCanControl(true);
        if (obstacleSpawner != null) obstacleSpawner.SetSpawning(true);
        if (roadScroller !=null) roadScroller.SetScrolling(true);
    }

    public void PauseGame()
    {
        if (State != GameState.Running) return;
        if (roadScroller != null) roadScroller.SetScrolling(false);
        SetState(GameState.Paused);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused) return;
        if (roadScroller != null) roadScroller.SetScrolling(true);
        Time.timeScale = 1f;
        SetState(GameState.Running);
    }

    public void LoseLife()
    {
        if (State != GameState.Running) return;
        Lives = Mathf.Max(0, Lives - 1);
        RefreshHud();

        if (Lives == 0)
        {
            if (roadScroller != null) roadScroller.SetScrolling(false);
            EndGame();
        }
        else if (statusText != null)
        {
            statusText.text = "Осталось жизней: " + Lives;
        }
    }

    public void AddScore(float amount)
    {
        Score = Mathf.Max(0f, Score + amount);
        RefreshHud();
    }

    public void AddLife(int amount = 1)
    {
        Lives = Mathf.Clamp(Lives + amount, 0, startingLives);
        RefreshHud();
    }

    private void EndGame()
    {
        SetState(GameState.GameOver);
        if (player != null) player.SetCanControl(false);
        if (obstacleSpawner != null) obstacleSpawner.SetSpawning(false);
        if (statusText != null) statusText.text = "Игра окончена";
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    private void SetState(GameState newState)
    {
        State = newState;
        if (startPanel != null) startPanel.SetActive(newState == GameState.Ready);
        if (pausePanel != null) pausePanel.SetActive(newState == GameState.Paused);
        if (gameOverPanel != null) gameOverPanel.SetActive(newState == GameState.GameOver);
        if (statusText != null && newState == GameState.Ready)
            statusText.text = "Нажмите «Старт»";
    }

    private void RefreshHud()
    {
        if (scoreText != null) scoreText.text = "Счёт: " + Mathf.FloorToInt(Score);
        if (livesText != null) livesText.text = "Жизни: " + Lives;
    }
}
