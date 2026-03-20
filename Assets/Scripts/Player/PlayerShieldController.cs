using System.Collections;
using UnityEngine;

public class PlayerShieldController : MonoBehaviour
{
    public Sprite blueSprite;
    public Sprite redSprite;
    public float cooldown = 5f;
    public float defaultKnockbackForce = 5;
    
    private float knockbackForce;
    [SerializeField]private bool isBlue;
    private string initialDirection;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        // condition ? if true : if false
        spriteRenderer.sprite = isBlue ? blueSprite : redSprite;
        knockbackForce = defaultKnockbackForce;

        initialDirection = transform.parent.name;
        if (initialDirection.Equals("L"))
        {
            spriteRenderer.flipX = true;
            transform.localPosition = new Vector3(-1, 0, 0);
        }
        else if (initialDirection.Equals("U"))
        {
            transform.Rotate(0, 0, 90);
            transform.localPosition = new Vector3(0, 1, 0);
        }
        else if (initialDirection.Equals("D"))
        {
            transform.Rotate(0, 0, -90);
            transform.localPosition = new Vector3(0, -1, 0);
        }
        else
        {
            transform.localPosition = new Vector3(1, 0, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Collision");
            EnemyData enemyData = collision.GetComponent<EnemyData>();
            if (enemyData != null && !enemyData.IsDead)
            {
                Debug.Log("In deep");
                enemyData.TakeKnockback(transform.right, knockbackForce);
                spriteRenderer.enabled = false;
                col.enabled = false;
                StartCoroutine(resetCooldown());
            }
        }
        else if (collision.CompareTag("Bullet"))
        {
            // code here
        }
    }

    IEnumerator resetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        spriteRenderer.enabled = true;
        col.enabled = true;
    }
}
