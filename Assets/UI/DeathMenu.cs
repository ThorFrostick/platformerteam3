using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    public void OnRetryClicked()
    {
        GameManager.Instance.RestartLevel();
    }

    public void OnMainMenuClicked()
    {
        GameManager.Instance.BackToMainMenu();
    }

    public void OnLeaderboardClicked()
    {
        GameManager.Instance.GoToLeaderboard();
    }
}

