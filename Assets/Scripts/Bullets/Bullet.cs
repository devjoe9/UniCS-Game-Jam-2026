using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        BlueBullet,
        RedBullet,
        OrangeBullet
    }

    [Header("Default Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private BulletType bulletType = BulletType.RedBullet;

    private Vector2 direction = Vector2.up;
    private float timer;
    private SpriteRenderer spriteRenderer;

    public void Initialize(Vector2 newDirection, float newSpeed, float newLifetime, BulletType newBulletType)
    {
        direction = newDirection.normalized;
        speed = newSpeed;
        lifetime = newLifetime;
        bulletType = newBulletType;

        timer = 0f;
        UpdateVisuals();
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisuals();
    }

    private void OnEnable()
    {
        timer = 0f;
        UpdateVisuals();
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        switch (bulletType)
        {
            case BulletType.BlueBullet:
                spriteRenderer.color = Color.blue;
                break;

            case BulletType.RedBullet:
                spriteRenderer.color = Color.red;
                break;

            case BulletType.OrangeBullet:
                spriteRenderer.color = Color.orange;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by bullet");
            Destroy(gameObject);
        }

        // For future player gun shots
    }
}