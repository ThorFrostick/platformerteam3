using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Game has quit");
    }
}
