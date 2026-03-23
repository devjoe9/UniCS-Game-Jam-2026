using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        BlueBullet,
        RedBullet,
        OrangeBullet
    }

    public BulletType Type => bulletType;

    [Header("Default Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float bulletSize = 1f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 100;

    [SerializeField] private BulletType bulletType = BulletType.RedBullet;

    private Vector2 direction = Vector2.up;
    private float timer;
    private SpriteRenderer spriteRenderer;

    public void Initialize(Vector2 newDirection, float newSpeed, float newLifetime, BulletType newBulletType, float? newBulletSize = null)
    {
        direction = newDirection.normalized;
        speed = newSpeed;
        lifetime = newLifetime;
        bulletType = newBulletType;
        if (newBulletSize.HasValue)
        {
            bulletSize = newBulletSize.Value;
        }

        transform.localScale = Vector3.one * bulletSize;

        timer = 0f;
        UpdateVisuals();
    }

    private void Awake()
    {
        transform.localScale = Vector3.one * bulletSize;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisuals();
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one * bulletSize;
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
                spriteRenderer.color = new Color(0.4f, 0.6f, 0.9f);
                break;

            case BulletType.RedBullet:
                spriteRenderer.color = new Color(1f, 0.5f, 0.5f);
                break;

            case BulletType.OrangeBullet:
                spriteRenderer.color = new Color(1f, 0.7f, 0.4f);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerData playerData = other.GetComponentInParent<PlayerData>();
            PlayerController playerController = other.GetComponentInParent<PlayerController>();

            if (playerData != null && playerData.IsVulnerable)
            {
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                playerData.TakeDamage(damage);
                playerController.TakeKnockback(knockbackDir, knockbackForce);
                Destroy(gameObject);
            }
        }
         if (other.CompareTag("border"))
    {
        Destroy(gameObject);
    }
    }
    
}
