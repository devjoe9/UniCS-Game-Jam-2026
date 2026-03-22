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
        public AudioClip effectAudio;
        public int spawnCount = 8;
        public Color neonColor = Color.cyan;
        [Range(0f, 10f)]
        public float glowIntensity = 2.5f;
        public float minScale = 50f;
        public float maxScale = 100f;
        
        [Header("Expanding Circle Settings (Optional)")]
        public bool useExpandingCircles = false;
        public float circleExpandSpeed = 200f;
        public float circleMaxSize = 800f;
        public float circleSpawnInterval = 0.5f;
        
        [Header("Orbital Beam Settings (Optional)")]
        public bool useOrbitalBeam = false;
        public Sprite beamParticleSprite;
        public RuntimeAnimatorController orbAnimatorController;
        public float orbSize = 150f;
        public float orbOffset = 300f;
        public float beamParticleSize = 30f;
        public int particlesPerBeam = 30;
        public float beamSpeed = 0.5f;
        public float beamPauseDuration = 0.3f;
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
    public float warningDuration = 3f;
    
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
    public float blinkSpeed = 0.2f;
    
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
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
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
        
        if (mode.effectAudio != null && audioSource != null)
        {
            audioSource.clip = mode.effectAudio;
            audioSource.volume = audioVolume;
            audioSource.Play();
        }
        
        if (mode.useExpandingCircles)
        {
            StartCoroutine(SpawnExpandingCircles(mode));
            
            float normalDuration = effectDuration - warningDuration;
            if (normalDuration > 0)
            {
                yield return new WaitForSeconds(normalDuration);
            }
        }
        else if (mode.useOrbitalBeam)
        {
            SpawnOrbitalBeam(mode);
            
            float normalDuration = effectDuration - warningDuration;
            if (normalDuration > 0)
            {
                yield return new WaitForSeconds(normalDuration);
            }
        }
        else
        {
            SpawnCanvasEffects(mode);
            
            float normalDuration = effectDuration - warningDuration;
            if (normalDuration > 0)
            {
                yield return new WaitForSeconds(normalDuration);
            }
        }
        
        if (warningDuration > 0)
        {
            yield return StartCoroutine(BlinkWarning());
        }
        
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        ClearEffects();
        
        isEffectActive = false;
        Debug.Log($"{mode.modeName} visual effect ended!");
    }
    
    IEnumerator SpawnExpandingCircles(ModeVisualEffect mode)
    {
        if (mode.effectSprite == null || effectCanvas == null)
        {
            Debug.LogWarning("Missing sprite or canvas for expanding circles!");
            yield break;
        }
        
        float elapsed = 0f;
        float totalDuration = effectDuration - warningDuration;
        
        while (elapsed < totalDuration && isEffectActive)
        {
            GameObject circleObj = new GameObject($"{mode.modeName}_Circle");
            circleObj.transform.SetParent(effectCanvas.transform, false);
            
            RectTransform rectTransform = circleObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            
            UnityEngine.UI.Image image = circleObj.AddComponent<UnityEngine.UI.Image>();
            image.sprite = mode.effectSprite;
            image.type = UnityEngine.UI.Image.Type.Simple;
            
            CanvasCirclePulse pulse = circleObj.AddComponent<CanvasCirclePulse>();
            pulse.Initialize(mode.circleMaxSize, mode.circleExpandSpeed, mode.neonColor, mode.glowIntensity);
            
            activeEffects.Add(circleObj);
            
            yield return new WaitForSeconds(mode.circleSpawnInterval);
            elapsed += mode.circleSpawnInterval;
        }
    }
    
    void SpawnOrbitalBeam(ModeVisualEffect mode)
    {
        if (mode.effectSprite == null || mode.beamParticleSprite == null || effectCanvas == null)
        {
            Debug.LogWarning("Missing sprites or canvas for orbital beam!");
            return;
        }
        
        GameObject beamObj = new GameObject($"{mode.modeName}_OrbitalBeam");
        beamObj.transform.SetParent(effectCanvas.transform, false);
        
        RectTransform rect = beamObj.AddComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        
        CanvasOrbitalBeam orbital = beamObj.AddComponent<CanvasOrbitalBeam>();
        orbital.Initialize(canvasRect, mode.effectSprite, mode.beamParticleSprite, mode.neonColor, mode.glowIntensity, mode.orbAnimatorController);
        orbital.orbSize = mode.orbSize;
        orbital.orbOffset = mode.orbOffset;
        orbital.particleSize = mode.beamParticleSize;
        orbital.particlesPerBeam = mode.particlesPerBeam;
        orbital.beamSpeed = mode.beamSpeed;
        orbital.beamPauseDuration = mode.beamPauseDuration;
        
        orbital.StartEffect();
        
        activeEffects.Add(beamObj);
    }
    
    IEnumerator BlinkWarning()
    {
        float elapsed = 0f;
        bool visible = true;
        
        while (elapsed < warningDuration)
        {
            visible = !visible;
            SetEffectsVisibility(visible);
            
            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;
        }
        
        SetEffectsVisibility(true);
    }
    
    void SetEffectsVisibility(bool visible)
    {
        foreach (GameObject effect in activeEffects)
        {
            if (effect != null)
            {
                UnityEngine.UI.Image[] images = effect.GetComponentsInChildren<UnityEngine.UI.Image>();
                foreach (UnityEngine.UI.Image image in images)
                {
                    if (image != null)
                    {
                        Color color = image.color;
                        color.a = visible ? 1f : 0f;
                        image.color = color;
                    }
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
                CanvasOrbitalBeam orbital = effect.GetComponent<CanvasOrbitalBeam>();
                if (orbital != null)
                {
                    orbital.StopEffect();
                }
                
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
            
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            
            ClearEffects();
            isEffectActive = false;
        }
    }
}