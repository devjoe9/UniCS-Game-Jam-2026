using UnityEngine;
using System.Collections.Generic;

public class CanvasImmortalityEffect : MonoBehaviour
{
    [Header("Angel Settings")]
    public Sprite angelSprite;
    public RuntimeAnimatorController angelAnimatorController;
    public Color angelColor = Color.white;
    public float angelSize = 150f;
    public float angelOffset = 300f;
    public float glowIntensity = 2f;
    
    private GameObject leftAngel;
    private GameObject rightAngel;
    
    public void Initialize(Sprite sprite, RuntimeAnimatorController animController, Color color, float intensity)
    {
        angelSprite = sprite;
        angelAnimatorController = animController;
        angelColor = color;
        glowIntensity = intensity;
    }
    
    public void StartEffect()
    {
        CreateAngels();
    }
    
    void CreateAngels()
    {
        leftAngel = CreateAngel(new Vector2(-angelOffset, 0));
        rightAngel = CreateAngel(new Vector2(angelOffset, 0));
    }
    
    GameObject CreateAngel(Vector2 position)
    {
        GameObject angel = new GameObject("Angel");
        angel.transform.SetParent(transform, false);
        
        RectTransform rect = angel.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(angelSize, angelSize);
        
        UnityEngine.UI.Image image = angel.AddComponent<UnityEngine.UI.Image>();
        image.sprite = angelSprite;
        image.color = angelColor * glowIntensity;
        
        if (angelAnimatorController != null)
        {
            Animator animator = angel.AddComponent<Animator>();
            animator.runtimeAnimatorController = angelAnimatorController;
        }
        
        return angel;
    }
    
    public List<GameObject> GetAllEffectObjects()
    {
        List<GameObject> all = new List<GameObject>();
        if (leftAngel != null) all.Add(leftAngel);
        if (rightAngel != null) all.Add(rightAngel);
        return all;
    }
    
    public void CleanUp()
    {
        if (leftAngel != null) Destroy(leftAngel);
        if (rightAngel != null) Destroy(rightAngel);
    }
}