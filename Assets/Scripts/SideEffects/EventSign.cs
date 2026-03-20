using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// EventSign
/// ──────────
/// Attach to EventSignPanel.
/// Call ShowSign(sprite, duration) from SideEffectDisplay
/// after the spin resolves.
///
/// Flickers 4 times then stays on for effect duration.
/// </summary>
public class EventSign : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public Image      signImage;

    [Header("Flicker Settings")]
    public int   flickerCount    = 4;
    public float flickerOnTime   = 0.1f;
    public float flickerOffTime  = 0.1f;

    private Coroutine activeCoroutine;


    /// <summary>
    /// Call this after spin resolves.
    /// sprite   = the event sign sprite
    /// duration = how long effect lasts (sign stays this long)
    /// </summary>
    public void ShowSign(Sprite sprite, float duration)
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(FlickerThenStay(sprite, duration));
    }

    public void HideSign()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        if (panel != null)
            panel.SetActive(false);
    }

    private IEnumerator FlickerThenStay(Sprite sprite, float duration)
    {
        if (signImage != null)
            signImage.sprite = sprite;

        // Flicker 4 times
        for (int i = 0; i < flickerCount; i++)
        {
            panel.SetActive(true);
            yield return new WaitForSeconds(flickerOnTime);
            panel.SetActive(false);
            yield return new WaitForSeconds(flickerOffTime);
        }

        // Stay on for effect duration
        panel.SetActive(true);
        yield return new WaitForSeconds(duration);

        // Hide
        panel.SetActive(false);
        activeCoroutine = null;
    }
}
