using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasNoWeaponsEffect : MonoBehaviour
{
    [Header("Cross Settings")]
    public Sprite crossSprite;
    public Color crossColor = Color.red;
    public float crossSize = 80f;
    public float crossOffset = 133f;
    public float glowIntensity = 2f;
    public float crossBlinkInterval = 0.5f;
    
    [Header("Warning Bar Settings")]
    public Sprite warningSprite;
    public int warningCount = 5;
    public float warningSize = 40f;
    public float warningSpacing = 50f;
    public Vector2 warningBarPosition = new Vector2(0, -250f);
    
    private List<GameObject> crosses = new List<GameObject>();
    private List<GameObject> warnings = new List<GameObject>();
    private RectTransform canvasRect;
    private Coroutine crossBlinkCoroutine;
    
    public void Initialize(RectTransform canvas, Sprite cross, Sprite warning, Color color, float intensity)
    {
        canvasRect = canvas;
        crossSprite = cross;
        warningSprite = warning;
        crossColor = color;
        glowIntensity = intensity;
    }
    
    public void StartEffect(float effectDuration, float warningDuration)
    {
        StartCoroutine(NoWeaponsEffectCoroutine(effectDuration, warningDuration));
    }
    
    IEnumerator NoWeaponsEffectCoroutine(float effectDuration, float warningDuration)
    {
        SpawnCrosses();
        SpawnWarningBar();
        
        // Start continuous blinking for crosses
        float normalDuration = effectDuration - warningDuration;
        crossBlinkCoroutine = StartCoroutine(BlinkCrossesContinuously(normalDuration));
        
        // Disappear warnings one by one from right to left
        float warningInterval = normalDuration / warningCount;
        
        for (int i = warningCount - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(warningInterval);
            
            if (i < warnings.Count && warnings[i] != null)
            {
                Destroy(warnings[i]);
                warnings[i] = null;
            }
        }
        
        // Stop the continuous blinking (last 3 seconds warning blink will take over)
        if (crossBlinkCoroutine != null)
        {
            StopCoroutine(crossBlinkCoroutine);
        }
        
        // Make sure crosses are visible for the final warning blink
        SetCrossesVisibility(true);
        
        Debug.Log("No Weapons effect - continuous blinking stopped, warning phase starting");
    }
    
    IEnumerator BlinkCrossesContinuously(float duration)
    {
        float elapsed = 0f;
        bool visible = true;
        
        while (elapsed < duration)
        {
            visible = !visible;
            SetCrossesVisibility(visible);
            
            yield return new WaitForSeconds(crossBlinkInterval);
            elapsed += crossBlinkInterval;
        }
        
        // Ensure visible at the end
        SetCrossesVisibility(true);
    }
    
    public void SetCrossesVisibility(bool visible)
    {
        foreach (GameObject cross in crosses)
        {
            if (cross != null)
            {
                UnityEngine.UI.Image img = cross.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                {
                    Color color = img.color;
                    color.a = visible ? 1f : 0f;
                    img.color = color;
                }
            }
        }
    }
    
    void SpawnCrosses()
    {
        crosses.Add(CreateCross(new Vector2(-crossOffset, crossOffset)));
        crosses.Add(CreateCross(new Vector2(crossOffset, crossOffset)));
        crosses.Add(CreateCross(new Vector2(-crossOffset, -crossOffset)));
        crosses.Add(CreateCross(new Vector2(crossOffset, -crossOffset)));
    }
    
    GameObject CreateCross(Vector2 position)
    {
        GameObject cross = new GameObject("Cross");
        cross.transform.SetParent(transform, false);
        
        RectTransform rect = cross.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(crossSize, crossSize);
        
        UnityEngine.UI.Image image = cross.AddComponent<UnityEngine.UI.Image>();
        image.sprite = crossSprite;
        image.color = crossColor * glowIntensity;
        
        return cross;
    }
    
    void SpawnWarningBar()
    {
        float totalWidth = (warningCount - 1) * warningSpacing;
        float startX = -totalWidth / 2f;
        
        for (int i = 0; i < warningCount; i++)
        {
            Vector2 position = warningBarPosition + new Vector2(startX + (i * warningSpacing), 0);
            GameObject warning = CreateWarning(position, i);
            warnings.Add(warning);
        }
    }
    
    GameObject CreateWarning(Vector2 position, int index)
    {
        GameObject warning = new GameObject($"Warning_{index}");
        warning.transform.SetParent(transform, false);
        
        RectTransform rect = warning.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(warningSize, warningSize);
        
        UnityEngine.UI.Image image = warning.AddComponent<UnityEngine.UI.Image>();
        image.sprite = warningSprite;
        image.color = crossColor * glowIntensity;
        
        return warning;
    }
    
    public List<GameObject> GetAllEffectObjects()
    {
        List<GameObject> all = new List<GameObject>();
        all.AddRange(crosses);
        all.AddRange(warnings);
        return all;
    }
    
    public void CleanUp()
    {
        if (crossBlinkCoroutine != null)
        {
            StopCoroutine(crossBlinkCoroutine);
        }
        
        foreach (GameObject cross in crosses)
        {
            if (cross != null) Destroy(cross);
        }
        
        foreach (GameObject warning in warnings)
        {
            if (warning != null) Destroy(warning);
        }
        
        crosses.Clear();
        warnings.Clear();
    }
}