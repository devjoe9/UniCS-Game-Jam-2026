using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SideEffectDisplay : MonoBehaviour
{
    private static SideEffectDisplay instance;
    public static SideEffectDisplay Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<SideEffectDisplay>();
            }
            return instance;
        }
    }

    [Header("UI References")]
    public GameObject      panel;
    public Image           eventIcon;
    public Image           frameImage;
    public TextMeshProUGUI eventLabel;
    public TextMeshProUGUI eventDescription;
    public GameObject gameManager;

    [Header("Event Icons (5 - in order)")]
    public Sprite[] eventIcons = new Sprite[5];

    [Header("Event Sign")]
    public EventSign eventSign;
    public Sprite[]  signSprites  = new Sprite[5];
    public float     effectDuration = 8f;

    [Header("Event Data")]
    public string[] eventNames = new string[]
    {
        "NO WEAPONS",
        "IMMORTALITY",
        "SLOW TIME",
        "KNOCKBACK",
        "SLIPPERY"
    };

    public string[] eventDescriptions = new string[]
    {
        "All weapons are hacked!",
        "You cannot be harmed!",
        "Time slows to a crawl...",
        "Enemies get knocked back!",
        "Movement is slippery!"
    };

    public Color[] eventColors = new Color[]
    {
        new Color(1f,   0.15f, 0.15f),   // red    - no weapons
        new Color(1f,   0.85f, 0f),      // yellow - immortality
        new Color(0.2f, 0.6f,  1f),      // blue   - slow time
        new Color(1f,   0.55f, 0.1f),    // orange - knockback
        new Color(0.2f, 1f,    0.35f),   // green  - slippery
    };

    [Header("Timing")]
    public float startInterval  = 0.08f;
    public float endInterval    = 0.5f;
    public float spinDuration   = 4f;
    public float holdDuration   = 3f;
    public float visualEffectDelay = 2f; // Delay before visual effects start

    [Header("Audio")]
    public AudioSource spinAudioSource;
    public AudioSource resultAudioSource;
    public AudioClip   spinSound;
    public AudioClip   positiveSound;
    public AudioClip   negativeSound;
    public AudioClip   neutralSound;

    [Header("Visual Effects")]
    public CanvasEffectManager canvasEffectManager;

    // 0=positive 1=neutral 2=negative
    // Order: No Weapons, Immortality, Slow Time, Knockback, Slippery
    private int[] eventCategory = new int[] { 2, 0, 1, 1, 1 };

    private bool isAnimating = false;
    private SideEffectsManager sideEffectsManager;

    void Start()
    {
        sideEffectsManager = FindAnyObjectByType<SideEffectsManager>();
        if (sideEffectsManager == null)
        {
            sideEffectsManager = gameManager.GetComponent<SideEffectsManager>();
        }

        // Find CanvasEffectManager if not assigned
        if (canvasEffectManager == null)
        {
            canvasEffectManager = FindAnyObjectByType<CanvasEffectManager>();
            if (canvasEffectManager == null)
            {
                Debug.LogError("[SideEffectDisplay] CanvasEffectManager not found in scene!");
            }
        }
    }

    public void TriggerRandomEvent()
    {
        if (isAnimating)
        {
            eventSign.HideSign();
            sideEffectsManager.StopCurrentEffect();
        }
        int finalIndex = Random.Range(0, eventNames.Length);
        StartCoroutine(StartRandomEvent(finalIndex));
        // StartCoroutine(Spin(finalIndex));
    }

    private IEnumerator StartRandomEvent(int finalIndex)
    {
        isAnimating = true;
        panel.SetActive(true);

        PlayResultSound(finalIndex);

        // Show event sign
        if (eventSign != null && finalIndex < signSprites.Length)
            eventSign.ShowSign(signSprites[finalIndex], effectDuration, visualEffectDelay);

        yield return new WaitForSeconds(visualEffectDelay);
        StartCoroutine(sideEffectsManager.StartSideEffect(finalIndex));

        yield return new WaitForSeconds(holdDuration);

        panel.SetActive(false);
        isAnimating = false;
    }

    private IEnumerator FadeOutSpin()
    {
        if (spinAudioSource == null) yield break;
        float dur = 0.3f; float e = 0f;
        float startVol = spinAudioSource.volume;
        while (e < dur)
        {
            e += Time.deltaTime;
            spinAudioSource.volume = Mathf.Lerp(startVol, 0f, e / dur);
            yield return null;
        }
        spinAudioSource.Stop();
        spinAudioSource.volume = 1f;
    }

    private void PlayResultSound(int index)
    {
        if (resultAudioSource == null) return;
        int category = eventCategory[index];
        AudioClip clip = category == 0 ? positiveSound :
                         category == 2 ? negativeSound :
                         neutralSound;
        if (clip != null)
            resultAudioSource.PlayOneShot(clip);
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
}