using System.Collections;
using UnityEngine;

public class HealerData : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    public float lifetime = 5f;
    public float flashAfterTime = 3f;
    public float flashInterval = 0.2f;
    public Color defaultColour = new Color(1f, 0.922f, 0.016f, 1f);
    public Color flashColour = new Color(1f, 0.922f, 0.016f, 0.5f);
    public float healing = 1;
    private bool isActive;
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isActive = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(DestroyHealer(lifetime, flashAfterTime));
    }

    // Update is called once per frame
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
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
