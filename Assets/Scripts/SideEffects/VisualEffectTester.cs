using UnityEngine;

public class VisualEffectTester : MonoBehaviour
{
    private CanvasEffectManager canvasManager;
    
    void Start()
    {
        canvasManager = FindAnyObjectByType<CanvasEffectManager>();
        
        if (canvasManager == null)
        {
            Debug.LogError("CanvasEffectManager not found in scene!");
        }
    }
    
    void Update()
    {
        if (canvasManager == null) return;
        
        // Press 1-5 to test each mode
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            canvasManager.StartNoWeaponsMode();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            canvasManager.StartImmortalityMode();
        }
            
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            canvasManager.StartTimeSlowedDownMode();
        }
            
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            canvasManager.StartKnockbackIncreasedMode();
        }
            
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            canvasManager.StartSlipperyMode();
        }
        
        // Press ESC to stop current effect
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvasManager.StopCurrentEffect();
        }
    }
}