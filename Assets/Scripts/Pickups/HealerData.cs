using System.Collections;
using UnityEngine;

public class HealerData : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip despawnSound;
    [Range(0f, 1f)] [SerializeField] private float pickupVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float despawnVolume = 1f;
    [SerializeField] private Canvas effectCanvas;

    public float lifetime = 5f;
    public float flashAfterTime = 3f;
    public float flashInterval = 0.2f;
    public Color defaultColour = new Color(1f, 0.922f, 0.016f, 1f);
    public Color flashColour = new Color(1f, 0.922f, 0.016f, 0.5f);
    public float healing = 1;

    private bool isActive;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    void Start()
    {
        isActive = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }

        StartCoroutine(DestroyHealer(lifetime, flashAfterTime));
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData playerData = collision.GetComponent<PlayerData>();
            if (playerData != null && !playerData.isMaxHealth())
            {
                playerData.TakeHealing(healing);
                isActive = false;
                SideEffectDisplay.Instance.TriggerRandomEvent();

                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);
                }

                Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DestroyHealer(float lifetime, float flashAfterTime)
    {
        yield return new WaitForSeconds(flashAfterTime);

        float elapsedTime = 0f;
        while (elapsedTime < lifetime - flashAfterTime && isActive)
        { 
            spriteRenderer.color = (spriteRenderer.color == defaultColour) ? flashColour : defaultColour;
            elapsedTime += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }
        
        if (isActive)
        {
            if (despawnSound != null)
            {
                AudioSource.PlayClipAtPoint(despawnSound, transform.position, despawnVolume);
            }

            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}