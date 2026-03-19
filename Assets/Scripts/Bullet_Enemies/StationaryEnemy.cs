using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Behaviour")]
    [SerializeField] private float fleeRadius = 2f;
    [SerializeField] private float fleeSpeed = 1.5f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (player == null) return;

        Vector2 lookDir = player.position - transform.position;
        float rotZ = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    private void FixedUpdate()
    {
        if (player == null || rb == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < fleeRadius)
        {
            Vector2 fleeDir = (rb.position - (Vector2)player.position).normalized;
            rb.linearVelocity = fleeDir * fleeSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, player.position);
    }
}