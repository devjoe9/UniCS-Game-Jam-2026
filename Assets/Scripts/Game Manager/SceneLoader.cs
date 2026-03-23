using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void EnsureUnpaused()
    {
        if (PauseManager.isPaused)
        {
            Time.timeScale = 1f;
            PauseManager.isPaused = false;
        }
    }

    public void LoadGame()
    {
        EnsureUnpaused();
        SceneManager.LoadScene("THE GAME");
    }

    public void LoadMenu()
    {
        EnsureUnpaused();
        SceneManager.LoadScene("startMenu");
    }

    public void LoadGameOver()
    {
        EnsureUnpaused();
        SceneManager.LoadScene("GameOver");
    }

    public void LoadLoadOut()
    {
        EnsureUnpaused();
        SceneManager.LoadScene("LoadoutScene");
    }
}