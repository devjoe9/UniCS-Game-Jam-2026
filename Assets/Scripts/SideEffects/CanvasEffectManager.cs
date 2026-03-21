using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasEffectManager : MonoBehaviour
{
    [System.Serializable]
    public class ModeVisualEffect
    {
        public string modeName;
        public Sprite effectSprite;
        public AudioClip effectAudio;  // NEW: Audio for this effect
        public int spawnCount = 8;
        public Color neonColor = Color.cyan;
        [Range(0f, 10f)]
        public float glowIntensity = 2.5f;
        public float minScale = 50f;
        public float maxScale = 100f;
    }
    
    [Header("Visual Effects for Each Mode (In Wheel Order)")]
    public ModeVisualEffect noWeaponsEffect;
    public ModeVisualEffect immortalityEffect;
    public ModeVisualEffect timeSlowedDownEffect;
    public ModeVisualEffect knockbackIncreasedEffect;
    public ModeVisualEffect slipperyEffect;
    
    [Header("Canvas Settings")]
    public Canvas effectCanvas;
    
    [Header("Effect Settings")]
    public float effectDuration = 20f;
    public float warningDuration = 3f;  // Last 3 seconds blink warning
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float audioVolume = 0.5f;
    
    [Header("Movement Settings")]
    public bool enableMovement = true;
    public float minSpeed = 100f;
    public float maxSpeed = 300f;
    public float horizontalBias = 0.7f;
    
    [Header("Animation Settings")]
    public bool enablePulseAnimation = true;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.2f;
    
    [Header("Blink Warning Settings")]
    public float blinkSpeed = 0.2f;  // How fast to blink (seconds per blink)
    
    private List<GameObject> activeEffects = new List<GameObject>();
    private bool isEffectActive = false;
    private RectTransform canvasRect;
    private AudioSource audioSource;
    
    void Awake()
    {
        if (noWeaponsEffect == null) noWeaponsEffect = new ModeVisualEffect();
        if (immortalityEffect == null) immortalityEffect = new ModeVisualEffect();
        if (timeSlowedDownEffect == null) timeSlowedDownEffect = new ModeVisualEffect();
        if (knockbackIncreasedEffect == null) knockbackIncreasedEffect = new ModeVisualEffect();
        if (slipperyEffect == null) slipperyEffect = new ModeVisualEffect();
        
        noWeaponsEffect.modeName = "No Weapons";
        immortalityEffect.modeName = "Immortality";
        timeSlowedDownEffect.modeName = "Time Slowed Down";
        knockbackIncreasedEffect.modeName = "Knockback Increased";
        slipperyEffect.modeName = "Slippery";
        
        if (effectCanvas == null)
        {
            CreateEffectCanvas();
        }
        
        canvasRect = effectCanvas.GetComponent<RectTransform>();
        
        // Add AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;  // Loop the audio during effect
    }
    
    void CreateEffectCanvas()
    {
        GameObject canvasObj = new GameObject("EffectCanvas");
        effectCanvas = canvasObj.AddComponent<Canvas>();
        effectCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        effectCanvas.sortingOrder = 10;
        
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
    }
    
    public void StartNoWeaponsMode() => StartModeEffect(noWeaponsEffect);
    public void StartImmortalityMode() => StartModeEffect(immortalityEffect);
    public void StartTimeSlowedDownMode() => StartModeEffect(timeSlowedDownEffect);
    public void StartKnockbackIncreasedMode() => StartModeEffect(knockbackIncreasedEffect);
    public void StartSlipperyMode() => StartModeEffect(slipperyEffect);
    
    void StartModeEffect(ModeVisualEffect mode)
    {
        if (isEffectActive)
        {
            Debug.Log($"Effect already active! Stopping previous and starting {mode.modeName}");
            StopCurrentEffect();
        }
        
        isEffectActive = true;
        StartCoroutine(ModeEffectCoroutine(mode));
    }
    
    IEnumerator ModeEffectCoroutine(ModeVisualEffect mode)
    {
        Debug.Log($"{mode.modeName} visual effect started!");
        
        // Play audio if assigned
        if (mode.effectAudio != null && audioSource != null)
        {
            audioSource.clip = mode.effectAudio;
            audioSource.volume = audioVolume;
            audioSource.Play();
        }
        
        // Spawn visual effects
        SpawnCanvasEffects(mode);
        
        // Wait for most of the duration
        float normalDuration = effectDuration - warningDuration;
        if (normalDuration > 0)
        {
            yield return new WaitForSeconds(normalDuration);
        }
        
        // Start blinking warning for last 3 seconds
        if (warningDuration > 0)
        {
            yield return StartCoroutine(BlinkWarning());
        }
        
        // Stop audio
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        // Clean up
        ClearEffects();
        
        isEffectActive = false;
        Debug.Log($"{mode.modeName} visual effect ended!");
    }
    
    IEnumerator BlinkWarning()
    {
        float elapsed = 0f;
        bool visible = true;
        
        while (elapsed < warningDuration)
        {
            // Toggle visibility
            visible = !visible;
            SetEffectsVisibility(visible);
            
            // Wait for blink interval
            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;
        }
        
        // Make sure they're visible at the end
        SetEffectsVisibility(true);
    }
    
    void SetEffectsVisibility(bool visible)
    {
        foreach (GameObject effect in activeEffects)
        {
            if (effect != null)
            {
                UnityEngine.UI.Image image = effect.GetComponent<UnityEngine.UI.Image>();
                if (image != null)
                {
                    Color color = image.color;
                    color.a = visible ? 1f : 0f;  // Set alpha to 0 (invisible) or 1 (visible)
                    image.color = color;
                }
            }
        }
    }
    
    void SpawnCanvasEffects(ModeVisualEffect mode)
    {
        if (mode.effectSprite == null)
        {
            Debug.LogWarning($"No sprite assigned for {mode.modeName}! Skipping visual effect.");
            return;
        }
        
        if (effectCanvas == null || canvasRect == null)
        {
            Debug.LogError("Canvas not found!");
            return;
        }
        
        for (int i = 0; i < mode.spawnCount; i++)
        {
            GameObject effectObj = new GameObject($"{mode.modeName}_UI_{i}");
            effectObj.transform.SetParent(effectCanvas.transform, false);
            
            RectTransform rectTransform = effectObj.AddComponent<RectTransform>();
            
            UnityEngine.UI.Image image = effectObj.AddComponent<UnityEngine.UI.Image>();
            image.sprite = mode.effectSprite;
            image.color = mode.neonColor * mode.glowIntensity;
            
            float randomScale = Random.Range(mode.minScale, mode.maxScale);
            rectTransform.sizeDelta = new Vector2(randomScale, randomScale);
            
            float randomX = Random.Range(-canvasRect.rect.width / 2f, canvasRect.rect.width / 2f);
            float randomY = Random.Range(-canvasRect.rect.height / 2f, canvasRect.rect.height / 2f);
            rectTransform.anchoredPosition = new Vector2(randomX, randomY);
            
            rectTransform.rotation = Quaternion.identity;
            
            if (enableMovement)
            {
                CanvasEffectBounce bouncer = effectObj.AddComponent<CanvasEffectBounce>();
                bouncer.Initialize(minSpeed, maxSpeed, horizontalBias, canvasRect);
            }
            
            if (enablePulseAnimation)
            {
                CanvasEffectPulse pulse = effectObj.AddComponent<CanvasEffectPulse>();
                pulse.Initialize(pulseSpeed, pulseAmount, randomScale);
            }
            
            activeEffects.Add(effectObj);
        }
        
        Debug.Log($"Spawned {mode.spawnCount} canvas effects for {mode.modeName}");
    }
    
    void ClearEffects()
    {
        foreach (GameObject effect in activeEffects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
        activeEffects.Clear();
    }
    
    public void StopCurrentEffect()
    {
        if (isEffectActive)
        {
            StopAllCoroutines();
            
            // Stop audio
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            
            ClearEffects();
            isEffectActive = false;
        }
    }
}