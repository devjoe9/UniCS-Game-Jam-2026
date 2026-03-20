using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SideEffectDisplay : MonoBehaviour
{
    [Header("UI References")]
    public GameObject      panel;
    public Image           eventIcon;
    public Image           frameImage;
    public TextMeshProUGUI eventLabel;
    public TextMeshProUGUI eventDescription;

    [Header("Event Icons (5 - in order)")]
    public Sprite[] eventIcons = new Sprite[5];

    [Header("Event Data")]
    public string[] eventNames = new string[]
    {
        "IMMORTALITY",
        "KNOCKBACK",
        "SLOW TIME",
        "HACKED",
        "SLIPPERY"
    };

    public string[] eventDescriptions = new string[]
    {
        "You cannot be harmed!",
        "Enemies get knocked back!",
        "Time slows to a crawl...",
        "All weapons are hacked!",
        "Movement is slippery!"
    };

    public Color[] eventColors = new Color[]
    {
        new Color(1f,   0.15f, 0.15f),   // red    - immortality
        new Color(1f,   0.55f, 0.1f),    // orange - knockback
        new Color(0.2f, 0.6f,  1f),      // blue   - slow time
        new Color(1f,   0.55f, 0.1f),    // orange - hacked
        new Color(0.2f, 1f,    0.35f),   // green  - slippery
    };

    [Header("Timing")]
    public float startInterval  = 0.08f;   // fast  - 80ms per icon
    public float endInterval    = 0.5f;    // slow  - 500ms per icon
    public float spinDuration   = 4f;      // total spin time
    public float holdDuration   = 3f;      // how long result stays

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip   tickSound;
    public AudioClip   resultSound;

    private bool isAnimating = false;


    public void TriggerRandomEvent()
    {
        if (isAnimating) return;
        int finalIndex = Random.Range(0, eventNames.Length);
        StartCoroutine(Spin(finalIndex));
    }

    private IEnumerator Spin(int finalIndex)
    {
        isAnimating = true;
        panel.SetActive(true);

        float elapsed         = 0f;
        float timeSinceSwitch = 0f;
        int   currentIndex    = Random.Range(0, eventNames.Length);

        // ── spin phase ────────────────────────────────────────────────────────
        while (elapsed < spinDuration)
        {
            elapsed         += Time.deltaTime;
            timeSinceSwitch += Time.deltaTime;

            // Cubic ease — stays fast for a long time then slows sharply
            float t        = elapsed / spinDuration;
            float eased    = t * t * t;
            float interval = Mathf.Lerp(startInterval, endInterval, eased);

            if (timeSinceSwitch >= interval)
            {
                timeSinceSwitch = 0f;

                // Last 15% — steer toward final index
                if (elapsed > spinDuration * 0.85f)
                {
                    currentIndex = (currentIndex + 1) % eventNames.Length;
                    if (currentIndex == finalIndex && elapsed > spinDuration * 0.93f)
                        break;
                }
                else
                {
                    // Pure random during fast phase — avoid same icon twice
                    int next = Random.Range(0, eventNames.Length);
                    while (next == currentIndex && eventNames.Length > 1)
                        next = Random.Range(0, eventNames.Length);
                    currentIndex = next;
                }

                ShowIcon(currentIndex, false);
                PlayTick();
            }

            yield return null;
        }

        // ── result ────────────────────────────────────────────────────────────
        ShowIcon(finalIndex, true);
        PlaySound(resultSound);
        StartCoroutine(PunchScale(eventIcon.transform));
        StartCoroutine(FlashFrame(eventColors[finalIndex]));

        Debug.Log($"[SideEffectDisplay] Result: {eventNames[finalIndex]}");

        yield return new WaitForSeconds(holdDuration);

        panel.SetActive(false);
        isAnimating = false;
    }

    private void ShowIcon(int index, bool isFinal)
    {
        if (index < 0 || index >= eventNames.Length) return;

        if (eventIcon != null && index < eventIcons.Length)
        {
            eventIcon.sprite = eventIcons[index];
            eventIcon.color  = Color.white;
        }

        if (eventLabel != null)
        {
            eventLabel.text  = eventNames[index];
            eventLabel.color = isFinal ? eventColors[index] : new Color(1f, 1f, 1f, 0.5f);
        }

        if (eventDescription != null)
            eventDescription.text = isFinal ? eventDescriptions[index] : "";

        if (frameImage != null)
            frameImage.color = isFinal
                ? eventColors[index]
                : new Color(eventColors[index].r, eventColors[index].g, eventColors[index].b, 0.25f);
    }

    private IEnumerator PunchScale(Transform t)
    {
        float dur = 0.12f; float e = 0f;
        while (e < dur) { e += Time.deltaTime; t.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.3f, e / dur); yield return null; }
        e = 0f;
        while (e < dur) { e += Time.deltaTime; t.localScale = Vector3.Lerp(Vector3.one * 1.3f, Vector3.one, e / dur); yield return null; }
        t.localScale = Vector3.one;
    }

    private IEnumerator FlashFrame(Color color)
    {
        if (frameImage == null) yield break;
        float dur = 0.25f; float e = 0f;
        Color bright = new Color(Mathf.Min(color.r * 2f, 1f), Mathf.Min(color.g * 2f, 1f), Mathf.Min(color.b * 2f, 1f), 1f);
        while (e < dur) { e += Time.deltaTime; frameImage.color = Color.Lerp(bright, color, e / dur); yield return null; }
        frameImage.color = color;
    }

    private void PlayTick()
    {
        if (audioSource != null && tickSound != null)
            audioSource.PlayOneShot(tickSound, 0.35f);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
