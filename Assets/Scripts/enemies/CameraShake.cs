using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] float magnitude = 0.2f;
    [SerializeField] float shakeSpeed = 10f;
    [SerializeField] float duration = 0.2f;

    private float timer = 0f;
    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (timer > 0)
        {
            Vector2 offset = new Vector2(
                Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f
            ) * magnitude;

            transform.localPosition = originalPos + new Vector3(offset.x, offset.y, 0);

            timer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalPos;
        }
        Shake(duration, magnitude);
    }

    // 🔥 Call this to trigger shake
    public void Shake(float time, float strength)
    {
        duration = time;
        magnitude = strength;
        timer = duration;
    }
}