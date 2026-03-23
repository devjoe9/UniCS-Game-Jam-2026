using UnityEngine;
using UnityEngine.UI;

public class UILogoFloat : MonoBehaviour
{
    [SerializeField] float floatAmount = 15f;
    [SerializeField] float floatSpeed = 1f;

    [SerializeField] float glowIntensity = 0.3f;
    [SerializeField] float glowSpeed = 1.2f;

    RectTransform rt;
    Vector2 startPos;
    Image img;
    Color baseColor;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;

        // img = GetComponent<Image>();
        // baseColor = img.color;
    }

    void Update()
    {
        // float
        float y = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        rt.anchoredPosition = startPos + new Vector2(0, y);

        // glow pulse
        float glow = 1 + Mathf.Sin(Time.time * glowSpeed) * glowIntensity;
        // img.color = baseColor * glow;
    }
}