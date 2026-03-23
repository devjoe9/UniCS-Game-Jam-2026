using System;
using UnityEngine;

public class HealingOrb : MonoBehaviour
{
    [Header("Player Tag")]
    public string playerTag = "Player";

    [Header("Float Animation")]
    public float bobSpeed    = 2f;
    public float bobAmount   = 0.12f;
    public float rotateSpeed = 45f;

    public event Action OnPickedUp;

    private Vector3 startPos;
    private bool    pickedUp = false;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (pickedUp) return;

        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;
        if (!other.CompareTag(playerTag)) return;

        pickedUp = true;
        OnPickedUp?.Invoke();
        Destroy(gameObject);
    }
}
