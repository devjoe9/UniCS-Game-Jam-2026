using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image itemImage;
    public Button leftButton;
    public Button rightButton;

    [Header("Traversal List")]
    public List<Sprite> items = new List<Sprite>();

    [Header("Optional")]
    public bool wrapAround = true;
    public int startIndex = 0;

    private int currentIndex = 0;

    public int CurrentIndex => currentIndex;
    public Sprite CurrentSprite => (items != null && items.Count > 0) ? items[currentIndex] : null;

    private void Start()
    {
        if (leftButton != null)
            leftButton.onClick.AddListener(Previous);

        if (rightButton != null)
            rightButton.onClick.AddListener(Next);

        if (items == null || items.Count == 0)
        {
            if (itemImage != null)
                itemImage.enabled = false;
            return;
        }

        currentIndex = Mathf.Clamp(startIndex, 0, items.Count - 1);
        Refresh();
    }

    public void Next()
    {
        if (items == null || items.Count == 0)
            return;

        if (wrapAround)
        {
            currentIndex = (currentIndex + 1) % items.Count;
        }
        else
        {
            currentIndex = Mathf.Min(currentIndex + 1, items.Count - 1);
        }

        Refresh();
    }

    public void Previous()
    {
        if (items == null || items.Count == 0)
            return;

        if (wrapAround)
        {
            currentIndex = (currentIndex - 1 + items.Count) % items.Count;
        }
        else
        {
            currentIndex = Mathf.Max(currentIndex - 1, 0);
        }

        Refresh();
    }

    public void SetIndex(int index)
    {
        if (items == null || items.Count == 0)
            return;

        currentIndex = Mathf.Clamp(index, 0, items.Count - 1);
        Refresh();
    }

    private void Refresh()
    {
        if (itemImage == null)
        {
            return;
        }

        itemImage.enabled = true;
        itemImage.sprite = items[currentIndex];
        itemImage.preserveAspect = true;
    }
}