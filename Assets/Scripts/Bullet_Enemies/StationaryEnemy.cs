using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;

    [Header("Behaviour")]
    [SerializeField] float fleeRadius = 2f;
    [SerializeField] float fleeSpeed = 1.5f; // slow movement

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //  Always face player
        Vector2 lookDir = player.position - transform.position;
        float rotZ = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    void FixedUpdate()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < fleeRadius)
        {
            //  Move directly away (slowly)
            Vector2 fleeDir = (rb.position - (Vector2)player.position).normalized;
            rb.linearVelocity = fleeDir * fleeSpeed;
        }
        else
        {
            //  Stay still
            rb.linearVelocity = Vector2.zero;
        }
    }
// dont worry abt these its for debugging
    void OnDrawGizmos()
    {
        if (player == null) return;

        //  Flee radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);

        //  Facing line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, player.position);
    }
}