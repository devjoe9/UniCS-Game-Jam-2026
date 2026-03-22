using UnityEngine;

public class CanvasCirclePulse : MonoBehaviour
{
    public float maxScale = 500f;
    public float expandSpeed = 200f;
    public Color startColor = Color.cyan;
    public float glowIntensity = 2f;
    
    private RectTransform rectTransform;
    private UnityEngine.UI.Image image;
    private float currentSize = 0f;
    
    public void Initialize(float maxSize, float speed, Color color, float intensity)
    {
        maxScale = maxSize;
        expandSpeed = speed;
        startColor = color;
        glowIntensity = intensity;
    }
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<UnityEngine.UI.Image>();
        
        // Start tiny
        currentSize = 10f;
        rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
        
        // Set initial color
        if (image != null)
        {
            image.color = startColor * glowIntensity;
        }
    }
    
    void Update()
    {
        // Expand the circle
        currentSize += expandSpeed * Time.deltaTime;
        
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
        }
        
        // Fade out as it gets bigger
        if (image != null)
        {
            float alpha = 1f - (currentSize / maxScale);
            alpha = Mathf.Clamp01(alpha);
            
            Color color = startColor * glowIntensity;
            color.a = alpha;
            image.color = color;
        }
        
        // Destroy when too big
        if (currentSize >= maxScale)
        {
            Destroy(gameObject);
        }
    }
}