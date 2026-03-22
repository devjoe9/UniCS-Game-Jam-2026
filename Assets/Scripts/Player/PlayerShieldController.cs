using System.Collections;
using UnityEngine;

public class PlayerShieldController : MonoBehaviour
{
    public Sprite blueSprite;
    public Sprite redSprite;
    public float cooldown = 5f;
    public float defaultKnockbackForce = 5;
    
    private float knockbackForce;
    private bool isBlue;
    private string initialDirection;

    private SpriteRenderer spriteRenderer;
    private SideData sideData;
    private Collider2D col;
    //audio stuff
    public AudioClip shieldSound;
    [Range(0f, 1f)] public float shieldVolume = 1f;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        sideData = GetComponent<SideData>();
        audioSource = GetComponent<AudioSource>();
        isBlue = sideData.IsBlue;
        
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
            EnemyData enemyData = collision.GetComponentInParent<EnemyData>();
            HomingMelee homingEnemy = collision.GetComponentInParent<HomingMelee>();
            if (enemyData != null && !enemyData.IsDead && enemyData.IsBlue.Equals(isBlue))
            {
                Debug.Log("In deep");
                if (audioSource != null && shieldSound != null)
                {
                    audioSource.PlayOneShot(shieldSound, shieldVolume);
                }
                if (homingEnemy != null)
                {
                    enemyData.Die();
                }
                else
                {
                    enemyData.TakeKnockback(transform.right, knockbackForce);
                }
                spriteRenderer.enabled = false;
                col.enabled = false;
                StartCoroutine(resetCooldown());
            }
        }
        else if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            if (bullet == null) return;

            bool shieldIsBlue = sideData.IsBlue;
            bool bulletMatchesShield =
                (shieldIsBlue && bullet.Type == Bullet.BulletType.BlueBullet) ||
                (!shieldIsBlue && bullet.Type == Bullet.BulletType.RedBullet);

            if (bulletMatchesShield)
            {
                Debug.Log("blocked");
                if (audioSource != null && shieldSound != null)
                {
                    audioSource.PlayOneShot(shieldSound, shieldVolume);
                }
                Destroy(collision.gameObject);
                spriteRenderer.enabled = false;
                col.enabled = false;
                StartCoroutine(resetCooldown());
            }
        }
    }

    IEnumerator resetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        spriteRenderer.enabled = true;
        col.enabled = true;
    }
}
