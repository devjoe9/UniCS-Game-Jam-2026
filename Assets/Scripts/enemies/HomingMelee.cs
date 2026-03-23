using System.Collections;
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

    public float knockbackDrag = 5f;
    public float knockbackForce = 20f;
    public int damage = 1;

    private Rigidbody2D rb;
    private Vector3 spawnPosition;
    private EnemyData data;
    private Collider2D[] colliders;
    private SpriteRenderer sr;
    private PlayerController playerController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;
        data = GetComponent<EnemyData>();

        colliders = GetComponents<Collider2D>(); // 👈 get BOTH colliders
        sr = GetComponent<SpriteRenderer>();
        playerController = FindAnyObjectByType<PlayerController>();

        // Auto-find player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
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
        if (player == null) return;

        // 🧲 Knockback handling
        if (data != null && data.IsKnockedBack)
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
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;

        rb.linearVelocity += dir * acceleration * Time.fixedDeltaTime;
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, friction * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerData playerData = other.GetComponentInParent<PlayerData>();

            if (playerData != null && playerData.IsVulnerable)
            {
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                playerData.TakeDamage(damage);
                playerController.TakeKnockback(knockbackDir, knockbackForce);
            }

            if (data != null)
            {
                data.Die();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator RespawnRoutine()
    {
        // 💀 "Death"
        rb.linearVelocity = Vector2.zero;

        // Disable ALL colliders
        foreach (Collider2D col in colliders)
            col.enabled = false;

        // Hide sprite
        if (sr != null)
            sr.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        // 🔄 Respawn
        transform.position = spawnPosition;
        rb.linearVelocity = Vector2.zero;

        foreach (Collider2D col in colliders)
            col.enabled = true;

        if (sr != null)
            sr.enabled = true;
    }
}