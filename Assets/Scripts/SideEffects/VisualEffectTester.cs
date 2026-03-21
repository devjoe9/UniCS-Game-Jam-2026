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
        else
        {
            Debug.Log("CanvasEffectManager found! Press 1-5 to test effects.");
        }
    }
    
    void Update()
    {
        if (canvasManager == null) return;
        
        // Press 1-5 to test each mode
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Testing No Weapons Mode (Key 1)");
            canvasManager.StartNoWeaponsMode();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Testing Immortality Mode (Key 2)");
            canvasManager.StartImmortalityMode();
        }
            
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Testing Time Slowed Down Mode (Key 3)");
            canvasManager.StartTimeSlowedDownMode();
        }
            
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Testing Knockback Increased Mode (Key 4)");
            canvasManager.StartKnockbackIncreasedMode();
        }
            
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Testing Slippery Mode (Key 5)");
            canvasManager.StartSlipperyMode();
        }
        
        // Press ESC to stop current effect
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Stopping current effect");
            canvasManager.StopCurrentEffect();
        }
    }
}