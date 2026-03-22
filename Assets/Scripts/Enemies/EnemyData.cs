using System.Collections;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public Sprite blueSprite;
    public Sprite redSprite;
    public Sprite whiteFlashSprite;
    public float flashDuration = 0.1f;
    public float maxHealth;
    public float invulnerableTime;
    public float flashInterval = 0.2f;
    public Color defaultColour = Color.white;
    public Color flashColour = Color.white;

    private Rigidbody2D rb;
    private float curHealth;
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
    [SerializeField] private GameObject explosionPrefab;
    public bool IsBlue
    {
        get {return isBlue;}
        set {isBlue = value;}
    }
    private SpriteRenderer spriteRenderer;
    //audio stuff
    public AudioClip hurtSound;
    [Range(0f, 1f)] public float hurtVolume = 1f;
    private AudioSource audioSource;
    private int pointsValue;
    public int PointsValue
    {
        get => pointsValue;
        set => pointsValue = value;
    }
    private EnemySpawnController waveController;

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
        audioSource = GetComponent<AudioSource>();
        waveController = FindAnyObjectByType<EnemySpawnController>();
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

    public void TakeDamage(float damage)
    {
        if (!isDead && isVulnerable)
        {
            curHealth -= damage;
            StartCoroutine(DamageFlash());
            Debug.Log("enemy hurt cuh"); 
            if (audioSource != null && hurtSound != null)
            {
                audioSource.PlayOneShot(hurtSound, hurtVolume);
            }

            if (curHealth <= 0)
            {
                isDead = true;
                Die();
                Debug.Log("enemy died cuh"); 
                waveController.CurScore = pointsValue;
                Destroy(gameObject);
                return;
                // death
            }
            else
            {
                isVulnerable = false;
                StartCoroutine(InvulnerableTimer());
            }
        }
    }

    public void TakeHealing(float healing)
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

    IEnumerator DamageFlash()
    {
        if (spriteRenderer == null || whiteFlashSprite == null) yield break;

        spriteRenderer.sprite = whiteFlashSprite;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.sprite = isBlue ? blueSprite : redSprite;
    }

    private void Die()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
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
