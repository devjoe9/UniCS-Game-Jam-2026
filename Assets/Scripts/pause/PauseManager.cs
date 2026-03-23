using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool isPaused = false;

    [SerializeField] GameObject pauseOverlay;

    // void Start()
    // {
    //     pauseOverlay = GameObject.FindGameObjectWithTag("Pause");
    // }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
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