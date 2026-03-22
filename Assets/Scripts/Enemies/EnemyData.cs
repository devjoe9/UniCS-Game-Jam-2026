using System.Collections;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public Sprite blueSprite;
    public Sprite redSprite;
    public int maxHealth;
    public float invulnerableTime;
    public float flashInterval = 0.2f;
    public Color defaultColour = new Color(1f, 0.922f, 0.016f, 1f);
    public Color flashColour = new Color(1f, 0.922f, 0.016f, 0.5f);

    private Rigidbody2D rb;
    private int curHealth;
    public bool IsVulnerable => isVulnerable;
    private bool isVulnerable;
    public bool IsDead => isDead;
    private bool isDead;
    public bool IsKnockedBack
    {
        get => isKnockedBack;
        set => isKnockedBack = value;
    }
    private bool isKnockedBack;
    [SerializeField]private bool isBlue;
    public bool IsBlue
    {
        get {return isBlue;}
        set {isBlue = value;}
    }
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        int colour = Random.Range(0, 2);
        isBlue = colour == 0 ? true : false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        curHealth = maxHealth;
        isVulnerable = true;
        isDead = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = isBlue ? blueSprite : redSprite;
    }

    public void SetColour(bool isBlue)
    {
        this.isBlue = isBlue;
        spriteRenderer.sprite = isBlue ? blueSprite : redSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (!isDead && isVulnerable)
        {
            curHealth -= damage;

            if (curHealth <= 0)
            {
                isDead = true;
                Destroy(gameObject);
                // death
            }
            else
            {
                isVulnerable = false;
                StartCoroutine(InvulnerableTimer());
            }
        }
    }

    public void TakeHealing(int healing)
    {
        if (!isDead)
        {
            curHealth += healing;
        }
    }

    IEnumerator InvulnerableTimer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < invulnerableTime)
        {
            elapsedTime += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        isVulnerable = true;
    }

    public bool isMaxHealth()
    {
        return curHealth.Equals(maxHealth);
    }

    public void TakeKnockback(Vector2 direction, float force)
    {
        isKnockedBack = true;
        rb.linearVelocity = direction * force;
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
