using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// SceneLoader
/// ────────────
/// Attach to any GameObject.
/// Wire the Start button's OnClick to SceneLoader.LoadGameScene()
/// OR assign it to LoadoutSelectManager's startButton field.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Scene to load")]
    [Tooltip("Must exactly match the scene name in File → Build Settings")]
    public string gameSceneName = "GameScene";

    public void LoadGameScene()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogWarning("[SceneLoader] No scene name set!");
            return;
        }

        Debug.Log($"[SceneLoader] Loading scene: {gameSceneName}");
        SceneManager.LoadScene(gameSceneName);
    }
}
