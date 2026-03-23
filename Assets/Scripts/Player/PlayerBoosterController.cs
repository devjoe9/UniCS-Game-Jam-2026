using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoosterController : MonoBehaviour
{
    public Sprite blueSprite;
    public Sprite redSprite;
    public Sprite blueBoostingSprite;
    public Sprite redBoostingSprite;
    public float boostForce = 20f;

    public AudioClip boostLoopSound;
    [Range(0f, 1f)] public float boostLoopVolume = 1f;
    
    private Sprite defaultSprite;
    private Sprite defaultBoostingSprite;
    private bool isBlue;
    private string initialDirection;
    public Vector2 ThrustDirection => -(Vector2)transform.right;
    public bool IsDisabled => isDisabled;
    private bool isDisabled;
    private SpriteRenderer spriteRenderer;
    private SideData sideData;
    private ParticleSystem[] smokeParticles;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sideData = GetComponent<SideData>();
        isBlue = sideData.IsBlue;
        smokeParticles = GetComponentsInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();

        foreach (var smoke in smokeParticles)
        {
            smoke.Stop();
        }
        
        defaultSprite = isBlue ? blueSprite : redSprite;
        defaultBoostingSprite = isBlue ? blueBoostingSprite : redBoostingSprite;
        spriteRenderer.sprite = defaultSprite;
        isDisabled = false;

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
            audioSource.volume = boostLoopVolume;
            audioSource.clip = boostLoopSound;
        }

        initialDirection = transform.parent.name;
        if (initialDirection.Equals("L"))
        {
            transform.Rotate(0, 0, 180);
            transform.localPosition = new Vector3(-0.75f, 0, 0);
        }
        else if (initialDirection.Equals("U"))
        {
            transform.Rotate(0, 0, 90);
            transform.localPosition = new Vector3(0, 0.75f, 0);
        }
        else if (initialDirection.Equals("D"))
        {
            transform.Rotate(0, 0, -90);
            transform.localPosition = new Vector3(0, -0.75f, 0);
        }
        else
        {
            transform.localPosition = new Vector3(0.75f, 0, 0);
        }
    }

    void Update()
    {
        
    }

    public void UseBoostingSprite(bool isBoosting)
    {
        spriteRenderer.sprite = isBoosting ? defaultBoostingSprite : defaultSprite;

        if (isBoosting)
        {
            foreach (var smoke in smokeParticles)
            {
                smoke.Play();
            }

            if (audioSource != null && boostLoopSound != null && !audioSource.isPlaying)
            {
                audioSource.volume = boostLoopVolume;
                audioSource.Play();
            }
        }
        else
        {
            foreach (var smoke in smokeParticles)
            {
                smoke.Stop();
            }

            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}