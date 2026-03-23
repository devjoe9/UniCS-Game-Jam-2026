using System.Collections;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public float maxHealth;
    public float invulnerableTime;
    public float flashInterval = 0.2f;
    public Color defaultColour = new Color(1f, 1f, 1f, 1f);
    public Color flashColour = new Color(1f, 0f, 0f, 0.5f);

    public float CurHealth => curHealth;
    private float curHealth;
    public bool IsVulnerable
    {
        get => isVulnerable;
        set => isVulnerable = value;
    }
    private bool isVulnerable;
    public bool IsDead => isDead;
    private bool isDead;
    private SpriteRenderer spriteRenderer;
    //audio stuff
    public AudioClip hurtSound;
    [Range(0f, 1f)] public float hurtVolume = 1f;
    private AudioSource audioSource;
    private HealthBarUI healthBar;
    private Coroutine activeInvulnerability;
    public Coroutine ActiveInvulnerability => activeInvulnerability;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHealth = maxHealth;
        isVulnerable = true;
        isDead = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        healthBar = FindAnyObjectByType<HealthBarUI>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = defaultColour;
        }
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
            healthBar.UpdatePlayerHealthBar(this);
            if (audioSource != null && hurtSound != null)
            {
                audioSource.PlayOneShot(hurtSound, hurtVolume);
            }

            if (curHealth <= 0)
            {
                isDead = true;
                // death
                Debug.Log("you died cuh"); //remove this 
                int score = FindAnyObjectByType<EnemySpawnController>().curScore;

                PlayerPrefs.SetInt("FinalScore", score);

                int high = PlayerPrefs.GetInt("HighScore", 0);
                if (score > high)
                {
                    PlayerPrefs.SetInt("HighScore", score);
                }
                PlayerPrefs.Save();
                FindAnyObjectByType<SceneLoader>().LoadGameOver();

            }
            else
            {
                isVulnerable = false;
                activeInvulnerability = StartCoroutine(InvulnerableTimer());
            }
        }
    }

    public void TakeHealing(float healing)
    {
        if (!isDead)
        {
            curHealth = Mathf.Min(maxHealth, curHealth += healing);
            healthBar.UpdatePlayerHealthBar(this);
        }
    }

    IEnumerator InvulnerableTimer()
    {
        float elapsedTime = 0f;
        bool useFlashColour = true;

        while (elapsedTime < invulnerableTime)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = useFlashColour ? flashColour : defaultColour;
                useFlashColour = !useFlashColour;
            }

            yield return new WaitForSeconds(flashInterval);
            elapsedTime += flashInterval;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = defaultColour;
        }

        isVulnerable = true;
        activeInvulnerability = null;
    }

    public bool isMaxHealth()
    {
        return curHealth.Equals(maxHealth);
    }
}
