using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;

    [Header("Movement")]
    [SerializeField] float acceleration = 4f;
    [SerializeField] float maxSpeed = 2f;
    [SerializeField] float friction = 2.5f;

    [Header("Behaviour")]
    [SerializeField] float fleeRadius = 2f;
    [SerializeField] float fleeMultiplier = 1.5f; // speed boost when fleeing

    private Rigidbody2D rb;

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
    }

    void FixedUpdate()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < fleeRadius)
        {
            // 🏃 Move away smoothly
            Vector2 fleeDir = (rb.position - (Vector2)player.position).normalized;

            rb.linearVelocity += fleeDir * acceleration * Time.fixedDeltaTime;

            float targetSpeed = maxSpeed * fleeMultiplier;
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, targetSpeed);
        }

        // 🧊 Always apply friction (so it slowly comes to rest)
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, friction * Time.fixedDeltaTime);

        ApplySidewaysDrift(); // 🔥 optional but nice
    }

    void ApplySidewaysDrift()
    {
        Vector2 velocity = rb.linearVelocity;

        Vector2 forward = transform.up;
        Vector2 right = new Vector2(forward.y, -forward.x);

        Vector2 forwardVel = forward * Vector2.Dot(velocity, forward);
        Vector2 sidewaysVel = right * Vector2.Dot(velocity, right);

        // Lower = more slide
        sidewaysVel *= 0.75f;

        rb.linearVelocity = forwardVel + sidewaysVel;
    }

    // debugging gizmos
    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, player.position);
    }
}