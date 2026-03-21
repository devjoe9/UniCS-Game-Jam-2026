using UnityEngine;

public class CanvasEffectPulse : MonoBehaviour
{
    private float pulseSpeed;
    private float pulseAmount;
    private float baseSize;
    
    private RectTransform rectTransform;
    
    public void Initialize(float speed, float amount, float size)
    {
        pulseSpeed = speed;
        pulseAmount = amount;
        baseSize = size;
        
        rectTransform = GetComponent<RectTransform>();
    }
    
    void Update()
    {
        if (rectTransform == null) return;
        
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        float size = baseSize * pulse;
        rectTransform.sizeDelta = new Vector2(size, size);
    }
}