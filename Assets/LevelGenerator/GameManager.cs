using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject deathMenu;
    public string mainMenuSceneName = "MainMenu";

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // Check the game start state
        Time.timeScale = 1f;
        if (deathMenu) deathMenu.SetActive(false);
        isGameOver = false;

        ScoreManager.Instance.ResetScore();
        ScoreManager.Instance.SetTimeScoring(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Active when player dead
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Show dealth UI
        if (deathMenu) deathMenu.SetActive(true);

        // Stop the game
        Time.timeScale = 0f;
        ScoreManager.Instance.SetTimeScoring(false);

        // If using cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Leaderboard.Instance != null)
        {
            int finalScore = ScoreManager.Instance.Score;


            float finalTime = 0;
            int finalCompanions = 0;

            Leaderboard.Instance.ShowOnDeath(finalScore, finalTime, finalCompanions);
        }
    }

    public void RestartLevel()
    {

        Time.timeScale = 1f;
        ScoreManager.Instance.SetTimeScoring(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Restart the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        ScoreManager.Instance.SetTimeScoring(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (deathMenu) deathMenu.SetActive(false);

        ScoreManager.Instance.SetTimeScoring(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoToLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void UpdateNewPlayer()
    {
        Leaderboard.Instance.score = ScoreManager.Instance.Score;
        Leaderboard.Instance.time = ScoreManager.Instance.elapsedTime;
        Leaderboard.Instance.companions = ScoreManager.Instance.companions;
    }
}
