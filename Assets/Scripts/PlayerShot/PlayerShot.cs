using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private Color blueColour = new Color(0, 0, 0, 0);
    [SerializeField] private Color redColour = new Color(0, 0, 0, 0);

    private Vector2 direction = Vector2.right;
    private float timer;
    private SpriteRenderer spriteRenderer;
    private bool isBlue;

    public void Initialize(Vector2 newDirection, float newSpeed, int newDamage, float newKnockbackForce, bool isBlue)
    {
        direction = newDirection.normalized;
        speed = newSpeed;
        timer = 0f;
        damage = newDamage;
        knockbackForce = newKnockbackForce;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = isBlue ? blueColour : redColour;
        this.isBlue = isBlue;
    }

    private void OnEnable()
    {
        timer = 0f;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit object: " + collision.name);
        if (collision.CompareTag("Enemy"))
        {
            EnemyData enemyData = collision.GetComponent<EnemyData>();  
            if (enemyData != null && !enemyData.IsDead && enemyData.IsBlue.Equals(isBlue))
            {
                enemyData.TakeDamage(damage);
                Debug.Log("bluh damaged");
                enemyData.TakeKnockback(transform.right, knockbackForce);
                Destroy(gameObject);
            }
        }
    }
}