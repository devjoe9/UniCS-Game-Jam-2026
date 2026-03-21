using UnityEngine;

public class HomingMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;

    [Header("Movement")]
    [SerializeField] float acceleration = 10f;
    [SerializeField] float maxSpeed = 6f;
    [SerializeField] float friction = 3f;

    [Header("Respawn")]
    [SerializeField] float respawnTime = 2f;

    private Rigidbody2D rb;
    private Vector3 spawnPosition;
    private EnemyData data;
    public float knockbackDrag = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
        data = GetComponent<EnemyData>();
    }

    void Update()
    {
        if (player == null) return;

        // 👀 Face player
        Vector2 dir = player.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void FixedUpdate()
    {
         // Taking knockback
        if (data.IsKnockedBack)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, knockbackDrag * Time.fixedDeltaTime);

            if (rb.linearVelocity.magnitude < 0.1f)
            {
                rb.linearVelocity = Vector2.zero;
                data.IsKnockedBack = false;
            }

            return;
        }

        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 dir = ((Vector2)player.position - rb.position).normalized;

        rb.linearVelocity += dir * acceleration * Time.fixedDeltaTime;
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, friction * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 👾 "Delete" enemy
            gameObject.SetActive(false);

            // ⏱ Respawn after delay
            Invoke(nameof(Respawn), respawnTime);
        }
    }

    void Respawn()
    {
        transform.position = spawnPosition;
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(true);
    }
}