using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasOrbitalBeam : MonoBehaviour
{
    [Header("Orb Settings")]
    public Sprite orbSprite;
    public Sprite beamParticleSprite;
    public RuntimeAnimatorController orbAnimatorController;
    public Color orbColor = new Color(1f, 0.5f, 0f);
    public float orbSize = 80f;
    public float orbOffset = 300f;
    
    [Header("Beam Settings")]
    public int particlesPerBeam = 30;
    public float particleSize = 20f;
    public float beamSpeed = 0.5f;
    public float beamPauseDuration = 0.3f;
    public float particleFadeDuration = 0.5f;
    
    [Header("Glow Settings")]
    public float glowIntensity = 2f;
    
    private RectTransform canvasRect;
    private GameObject leftOrb;
    private GameObject rightOrb;
    private List<GameObject> activeParticles = new List<GameObject>();
    private bool isRunning = false;
    
    public void Initialize(RectTransform canvas, Sprite orb, Sprite particle, Color color, float intensity, RuntimeAnimatorController animController)
    {
        canvasRect = canvas;
        orbSprite = orb;
        beamParticleSprite = particle;
        orbColor = color;
        glowIntensity = intensity;
        orbAnimatorController = animController;
    }
    
    public void StartEffect()
    {
        if (isRunning) return;
        
        CreateOrbs();
        isRunning = true;
        StartCoroutine(BeamAnimationLoop());
    }
    
    public void StopEffect()
    {
        isRunning = false;
        StopAllCoroutines();
        
        if (leftOrb != null) Destroy(leftOrb);
        if (rightOrb != null) Destroy(rightOrb);
        
        foreach (GameObject particle in activeParticles)
        {
            if (particle != null) Destroy(particle);
        }
        activeParticles.Clear();
    }
    
    void CreateOrbs()
    {
        leftOrb = CreateOrb(new Vector2(-orbOffset, 0));
        rightOrb = CreateOrb(new Vector2(orbOffset, 0));
    }
    
    GameObject CreateOrb(Vector2 position)
    {
        GameObject orb = new GameObject("Orb");
        orb.transform.SetParent(transform, false);
        
        RectTransform rect = orb.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(orbSize, orbSize);
        
        UnityEngine.UI.Image image = orb.AddComponent<UnityEngine.UI.Image>();
        image.sprite = orbSprite;
        image.color = orbColor * glowIntensity;
        
        // Add animator for sprite animation
        if (orbAnimatorController != null)
        {
            Animator animator = orb.AddComponent<Animator>();
            animator.runtimeAnimatorController = orbAnimatorController;
        }
        
        return orb;
    }
    
    IEnumerator BeamAnimationLoop()
    {
        while (isRunning)
        {
            yield return StartCoroutine(AnimateBeam(
                new Vector2(-orbOffset, 0), 
                new Vector2(orbOffset, 0), 
                true
            ));
            
            yield return new WaitForSeconds(beamPauseDuration);
            
            yield return StartCoroutine(AnimateBeam(
                new Vector2(orbOffset, 0), 
                new Vector2(-orbOffset, 0), 
                false
            ));
            
            yield return new WaitForSeconds(beamPauseDuration);
        }
    }
    
    IEnumerator AnimateBeam(Vector2 startPos, Vector2 endPos, bool upperArc)
    {
        List<GameObject> currentBeamParticles = new List<GameObject>();
        
        for (int i = 0; i < particlesPerBeam; i++)
        {
            float t = (float)i / (particlesPerBeam - 1);
            Vector2 arcPosition = CalculateArcPosition(startPos, endPos, t, upperArc);
            
            GameObject particle = CreateBeamParticle(arcPosition);
            currentBeamParticles.Add(particle);
            activeParticles.Add(particle);
            
            yield return new WaitForSeconds(beamSpeed / particlesPerBeam);
        }
        
        StartCoroutine(FadeOutParticles(currentBeamParticles));
    }
    
    Vector2 CalculateArcPosition(Vector2 start, Vector2 end, float t, bool upperArc)
    {
        float x = Mathf.Lerp(start.x, end.x, t);
        float arcHeight = orbOffset * 0.8f;
        float y = Mathf.Sin(t * Mathf.PI) * arcHeight;
        
        if (!upperArc)
        {
            y = -y;
        }
        
        return new Vector2(x, y);
    }
    
    GameObject CreateBeamParticle(Vector2 position)
    {
        GameObject particle = new GameObject("BeamParticle");
        particle.transform.SetParent(transform, false);
        
        RectTransform rect = particle.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(particleSize, particleSize);
        
        UnityEngine.UI.Image image = particle.AddComponent<UnityEngine.UI.Image>();
        image.sprite = beamParticleSprite;
        image.color = orbColor * glowIntensity;
        
        return particle;
    }
    
    IEnumerator FadeOutParticles(List<GameObject> particles)
    {
        float elapsed = 0f;
        
        while (elapsed < particleFadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / particleFadeDuration);
            
            foreach (GameObject particle in particles)
            {
                if (particle != null)
                {
                    UnityEngine.UI.Image image = particle.GetComponent<UnityEngine.UI.Image>();
                    if (image != null)
                    {
                        Color color = image.color;
                        color.a = alpha;
                        image.color = color;
                    }
                }
            }
            
            yield return null;
        }
        
        foreach (GameObject particle in particles)
        {
            if (particle != null)
            {
                activeParticles.Remove(particle);
                Destroy(particle);
            }
        }
    }
}