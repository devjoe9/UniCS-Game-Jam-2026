using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool isPaused = false;

    [SerializeField] GameObject pauseOverlay;

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    void OnPause()
    {
        TogglePause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        pauseOverlay.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseOverlay.SetActive(false);
    }
}