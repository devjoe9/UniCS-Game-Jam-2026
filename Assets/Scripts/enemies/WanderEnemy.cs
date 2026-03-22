using UnityEngine;

public class WanderEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] Collider2D boundary;

    [Header("Movement")]
    [SerializeField] float acceleration = 6f;
    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float friction = 1.5f;

    [Header("Wander")]
    [SerializeField] float pointReachDistance = 0.2f;
    [SerializeField] float wanderRadius = 2f;

    [Header("Flee")]
    [SerializeField] float fleeRadius = 2f;
    [SerializeField] float fleeSpeedMultiplier = 1.8f;

    private Rigidbody2D rb;
    private Vector2 targetPoint;
    private EnemyData data;
    public float knockbackDrag = 5f;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (boundary == null)
            boundary = GameObject.FindGameObjectWithTag("Boundary")?.GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        PickNewPoint(false);
        data = GetComponent<EnemyData>();




    }

    void Update()
    {
        // 👀 Face player
        Vector2 lookDir = player.position - transform.position;
        float rotZ = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // 🏃 If player close → pick biased point
        if (distToPlayer < fleeRadius)
        {
            PickNewPoint(true);
        }
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

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        Vector2 dir = (targetPoint - rb.position);
        float targetSpeed = (distToPlayer < fleeRadius) ? maxSpeed * fleeSpeedMultiplier : maxSpeed;

        if (dir.magnitude > pointReachDistance)
        {
            Vector2 desiredDir = dir.normalized;

            // 🚀 Accelerate toward target
            rb.linearVelocity += desiredDir * acceleration * Time.fixedDeltaTime;

            // 🧱 Clamp speed
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, targetSpeed);
        }
        else
        {
            PickNewPoint(false);
        }

        // 🧊 Apply friction (controls slipperiness)
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, friction * Time.fixedDeltaTime);

        ApplySidewaysDrift(); // 🔥 drift feel
    }

    void ApplySidewaysDrift()
    {
        Vector2 velocity = rb.linearVelocity;

        Vector2 forward = transform.up;
        Vector2 right = new Vector2(forward.y, -forward.x);

        Vector2 forwardVel = forward * Vector2.Dot(velocity, forward);
        Vector2 sidewaysVel = right * Vector2.Dot(velocity, right);

        // 🔥 Lower = more drift
        sidewaysVel *= 0.7f;

        rb.linearVelocity = forwardVel + sidewaysVel;
    }

    void PickNewPoint(bool useBias)
    {
        Bounds bounds = boundary.bounds;
        Vector2 currentPos = transform.position;

        Vector2 randomOffset;

        if (useBias)
        {
            Vector2 awayFromPlayer = ((Vector2)transform.position - (Vector2)player.position).normalized;
            randomOffset = (awayFromPlayer + Random.insideUnitCircle * 0.5f).normalized * wanderRadius;
        }
        else
        {
            randomOffset = Random.insideUnitCircle * wanderRadius;
        }

        Vector2 newPoint = currentPos + randomOffset;

        // 🔒 Clamp inside boundary
        newPoint.x = Mathf.Clamp(newPoint.x, bounds.min.x, bounds.max.x);
        newPoint.y = Mathf.Clamp(newPoint.y, bounds.min.y, bounds.max.y);

        targetPoint = newPoint;
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetPoint, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, targetPoint);
    }
}