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
    private Coroutine activeCoroutine;


    /// <summary>
    /// Call this after spin resolves.
    /// sprite   = the event sign sprite
    /// duration = how long effect lasts (sign stays this long)
    /// </summary>
    public void ShowSign(Sprite sprite, float duration, float delay)
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = StartCoroutine(FlickerThenStay(sprite, duration, delay));
    }

    public void HideSign()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        if (panel != null)
            panel.SetActive(false);
    }

    private IEnumerator FlickerThenStay(Sprite sprite, float duration, float delay)
    {
        if (signImage != null)
            signImage.sprite = sprite;

        // flicker IN
        float timeElapsed = 0;
        bool flickerOn = false;
        while (timeElapsed < delay)
        {
            flickerOn = !flickerOn;
            panel.SetActive(flickerOn);
            yield return new WaitForSeconds(flickerOnTime);
            timeElapsed += flickerOnTime;
        };

        // don't flicker before flicker out
        float stayTime = Mathf.Max(0, duration - delay);
        panel.SetActive(true);
        yield return new WaitForSeconds(stayTime);

        // flicker out
        timeElapsed = 0;
        flickerOn = false;
        while (timeElapsed < delay)
        {
            flickerOn = !flickerOn;
            panel.SetActive(flickerOn);
            yield return new WaitForSeconds(flickerOnTime);
            timeElapsed += flickerOnTime;
        };

        // Final OFF
        panel.SetActive(false);
        activeCoroutine = null;
    }
}
