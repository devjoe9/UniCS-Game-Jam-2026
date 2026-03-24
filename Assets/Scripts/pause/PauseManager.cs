using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool isPaused = false;
    private SideEffectsManager sideEffectsManager;

    [SerializeField] GameObject pauseOverlay;

    void Start()
    {
        sideEffectsManager = FindFirstObjectByType<SideEffectsManager>();
        isPaused = false;
    }

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
        Time.timeScale = sideEffectsManager.CurTimeScale;
        isPaused = false;
        pauseOverlay.SetActive(false);
    }
}