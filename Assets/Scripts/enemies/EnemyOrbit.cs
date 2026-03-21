using UnityEngine;
using System.Collections;

public class EnemyOrbit : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float radius = 4f;
    [SerializeField] float speed = 4f;
    [SerializeField] float stoppingDistance = 0.3f;

    [SerializeField] float changeInterval = 2f;
    [SerializeField] float angleMoveSpeed = 120f;

    [SerializeField] float maxStartDelay = 1f;

    [SerializeField] float angleChangeRange = 90f;

    private float timer;

    private Rigidbody2D rb;
    private Vector2 targetPoint;

    private float angle;
    private float targetAngle;

    private bool canMove = false;
    private EnemyData data;

    public float knockbackDrag = 5f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        data = GetComponent<EnemyData>();

        angle = Random.Range(0f, Mathf.PI * 2f);
        targetAngle = angle;

        StartCoroutine(StartMoveDelay());
    }

    IEnumerator StartMoveDelay()
    {
        float delay = Random.Range(0f, maxStartDelay);
        yield return new WaitForSeconds(delay);

        canMove = true;
    }

    void Update()
    {
        if (!canMove) return;

        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            PickNewTargetAngle();
            timer = 0f;
        }

        float angleDeg = angle * Mathf.Rad2Deg;
        float targetDeg = targetAngle * Mathf.Rad2Deg;

        angleDeg = Mathf.MoveTowardsAngle(angleDeg, targetDeg, angleMoveSpeed * Time.deltaTime);
        angle = angleDeg * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        targetPoint = (Vector2)player.position + offset;

        Vector2 direction = player.position - transform.position;
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
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

        Vector2 direction = targetPoint - rb.position;

        if (direction.magnitude > stoppingDistance)
        {
            rb.linearVelocity = direction.normalized * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            PickNewTargetAngle();
        }
    }

    void PickNewTargetAngle()
    {
        float currentDeg = angle * Mathf.Rad2Deg;
        float newDeg = currentDeg + Random.Range(-angleChangeRange, angleChangeRange);
        targetAngle = newDeg * Mathf.Deg2Rad;
    }
    

    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(player.position, radius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPoint, 0.2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, targetPoint);
    }
}