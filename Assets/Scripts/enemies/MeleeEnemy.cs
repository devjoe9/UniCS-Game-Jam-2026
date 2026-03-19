using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;

    [Header("Movement")]
    [SerializeField] float acceleration = 8f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float friction = 2f;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDuration = 0.4f;
    [SerializeField] float dashCooldown = 2f;

    private Rigidbody2D rb;

    private bool isDashing = false;
    private float dashTimer;
    private float cooldownTimer;

    private Vector2 dashDirection;

    private bool canChainDash = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 👀 Always face player
        Vector2 lookDir = player.position - transform.position;
        float rotZ = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

        if (isDashing) return;

        // ⏱ cooldown timer
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= dashCooldown)
        {
            StartDash();
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            DashMovement();
        }
        else
        {
            IdleMovement();
        }

        ApplySidewaysDrift(); // 🔥 adds that smooth drift feel
    }

    void IdleMovement()
    {
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;

        // Accelerate toward player
        rb.linearVelocity += dir * acceleration * Time.fixedDeltaTime;

        // Clamp max speed
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);

        // Apply friction (controls slipperiness)
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, friction * Time.fixedDeltaTime);
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = 0f;
        cooldownTimer = 0f;

        // 🎯 Lock direction at start
        dashDirection = ((Vector2)player.position - rb.position).normalized;

        Object.FindFirstObjectByType<CameraShake>()?.Shake(0.2f, 0.3f);
    }

    void DashMovement()
    {
        dashTimer += Time.fixedDeltaTime;

        // Add force instead of overriding velocity
        rb.linearVelocity += dashDirection * dashSpeed * Time.fixedDeltaTime;

        // Clamp dash speed
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, dashSpeed);

        if (dashTimer >= dashDuration)
        {
            isDashing = false;

            // ❌ DO NOT zero velocity → keeps drift!
            canChainDash = true;
        }
    }

    void ApplySidewaysDrift()
    {
        Vector2 velocity = rb.linearVelocity;

        Vector2 forward = transform.up;
        Vector2 right = new Vector2(forward.y, -forward.x);

        Vector2 forwardVel = forward * Vector2.Dot(velocity, forward);
        Vector2 sidewaysVel = right * Vector2.Dot(velocity, right);

        // 🔥 Reduce sideways grip (lower = more drift)
        sidewaysVel *= 0.7f;

        rb.linearVelocity = forwardVel + sidewaysVel;
    }

    // 🔥 TRIGGER CHAIN DASH
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryChainDash();
        }
    }

    void TryChainDash()
    {
        if (!isDashing && cooldownTimer < dashCooldown && canChainDash)
        {
            StartDash();
            canChainDash = false;
        }
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, player.position);
    }
}