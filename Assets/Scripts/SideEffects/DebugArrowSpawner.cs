using UnityEngine;

public class DebugArrowSpawner : MonoBehaviour
{
    void Update()
    {
        // Find all arrows in the scene
        GameObject[] arrows = GameObject.FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in arrows)
        {
            if (obj.name.Contains("Slippery_Visual"))
            {
                Debug.Log($"Arrow found: {obj.name} at position {obj.transform.position}");
                
                // Draw a debug line to the arrow from camera
                Debug.DrawLine(Camera.main.transform.position, obj.transform.position, Color.green, 2f);
            }
        }
    }
}