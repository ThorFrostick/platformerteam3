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
        ResumeGame();
        if (deathMenu) deathMenu.SetActive(false);
        isGameOver = false;
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

        // If using cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartLevel()
    {
        
        ResumeGame();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ResumeGame()
    {
        // Must update the timeScale before restart
        Time.timeScale = 1f;
        if (deathMenu) deathMenu.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
