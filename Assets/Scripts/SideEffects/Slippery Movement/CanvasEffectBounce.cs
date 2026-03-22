using UnityEngine;

public class CanvasEffectBounce : MonoBehaviour
{
    private float minSpeed;
    private float maxSpeed;
    private float horizontalBias;
    private RectTransform canvasRect;
    
    private Vector2 velocity;
    private RectTransform rectTransform;
    
    public void Initialize(float min, float max, float hBias, RectTransform canvas)
    {
        minSpeed = min;
        maxSpeed = max;
        horizontalBias = hBias;
        canvasRect = canvas;
        
        rectTransform = GetComponent<RectTransform>();
        
        float horizontalSpeed = Random.Range(minSpeed, maxSpeed) * (Random.value > 0.5f ? 1f : -1f);
        float verticalSpeed = Random.Range(minSpeed * (1f - horizontalBias), maxSpeed * (1f - horizontalBias)) * (Random.value > 0.5f ? 1f : -1f);
        
        velocity = new Vector2(horizontalSpeed, verticalSpeed);
    }
    
    void Update()
    {
        if (rectTransform == null || canvasRect == null) return;
        
        rectTransform.anchoredPosition += velocity * Time.deltaTime;
        
        Vector2 pos = rectTransform.anchoredPosition;
        
        float halfWidth = canvasRect.rect.width / 2f;
        float halfHeight = canvasRect.rect.height / 2f;
        
        if (pos.x < -halfWidth || pos.x > halfWidth)
        {
            velocity.x = -velocity.x;
            pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);
        }
        
        if (pos.y < -halfHeight || pos.y > halfHeight)
        {
            velocity.y = -velocity.y;
            pos.y = Mathf.Clamp(pos.y, -halfHeight, halfHeight);
        }
        
        rectTransform.anchoredPosition = pos;
    }
}