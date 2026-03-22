using UnityEngine;

public class BackgroundOscillator : MonoBehaviour
{
    [Header("Oscillation")]
    [SerializeField] float xAmount = 0.2f;
    [SerializeField] float yAmount = 0.1f;

    [SerializeField] float xSpeed = 0.15f;
    [SerializeField] float ySpeed = 0.25f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * xSpeed) * xAmount;
        float y = Mathf.Sin(Time.time * ySpeed) * yAmount;

        transform.position = startPos + new Vector3(x, y, 0);
    }
}