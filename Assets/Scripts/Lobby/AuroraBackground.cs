using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AuroraBackground
/// ─────────────────
/// Attach to a Canvas or Panel GameObject.
/// Creates animated green aurora bands on a black background.
///
/// Setup:
///  1. Create a UI Panel as the background, set its color to black.
///  2. Create an empty child GameObject called "AuroraContainer".
///  3. Attach this script to the AuroraContainer GameObject.
///  4. Make sure AuroraContainer has a RectTransform that fills the screen.
/// </summary>
public class AuroraBackground : MonoBehaviour
{
    [Header("Aurora Settings")]
    [Range(3, 10)]
    public int bandCount = 6;

    [Range(50f, 400f)]
    public float bandHeight = 180f;

    [Range(0.1f, 1f)]
    public float moveSpeed = 0.25f;

    [Range(0.5f, 3f)]
    public float pulseSpeed = 1.2f;

    [Range(0f, 1f)]
    public float maxAlpha = 0.18f;

    public Color auroraColor = new Color(0f, 1f, 0.4f, 1f);

    [Header("Screen Size (leave 0 to auto-detect)")]
    public float screenWidth  = 0f;
    public float screenHeight = 0f;

    // ── runtime ───────────────────────────────────────────────────────────────

    private struct Band
    {
        public RectTransform rect;
        public Image         image;
        public float         yOffset;      // current vertical position
        public float         ySpeed;       // scroll speed multiplier
        public float         pulseOffset;  // phase offset for alpha pulse
        public float         pulseRate;    // individual pulse speed
        public float         skew;         // horizontal drift
    }

    private List<Band> bands = new List<Band>();
    private float W, H;

    private void Awake()
    {
        W = screenWidth  > 0 ? screenWidth  : Screen.width;
        H = screenHeight > 0 ? screenHeight : Screen.height;

        SpawnBands();
    }

    private void SpawnBands()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        bands.Clear();

        for (int i = 0; i < bandCount; i++)
        {
            // Create GameObject
            GameObject go = new GameObject($"AuroraBand_{i}", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(transform, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.pivot     = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(W * Random.Range(1.2f, 1.8f), bandHeight * Random.Range(0.6f, 1.4f));

            Image img = go.GetComponent<Image>();
            img.color = new Color(auroraColor.r, auroraColor.g, auroraColor.b, 0f);

            // Randomise initial state
            float startY = Random.Range(-H * 0.1f, H * 1.1f);

            Band b = new Band
            {
                rect        = rt,
                image       = img,
                yOffset     = startY,
                ySpeed      = Random.Range(0.4f, 1.0f),
                pulseOffset = Random.Range(0f, Mathf.PI * 2f),
                pulseRate   = Random.Range(0.6f, 1.4f),
                skew        = Random.Range(-W * 0.3f, W * 0.3f)
            };

            bands.Add(b);
            UpdateBandTransform(b);
        }
    }

    private void Update()
    {
        W = screenWidth  > 0 ? screenWidth  : Screen.width;
        H = screenHeight > 0 ? screenHeight : Screen.height;

        float t = Time.time;

        for (int i = 0; i < bands.Count; i++)
        {
            Band b = bands[i];

            // Drift upward slowly
            b.yOffset += moveSpeed * b.ySpeed * Time.deltaTime * H * 0.1f;

            // Wrap around when fully off screen top
            if (b.yOffset > H * 1.15f)
                b.yOffset = -bandHeight * 1.5f;

            // Pulse alpha with sine wave
            float alpha = maxAlpha * 0.5f
                        * (1f + Mathf.Sin(t * pulseSpeed * b.pulseRate + b.pulseOffset));

            // Horizontal shimmer
            float xShimmer = Mathf.Sin(t * 0.4f + b.pulseOffset) * W * 0.05f;

            b.image.color = new Color(
                auroraColor.r,
                auroraColor.g,
                auroraColor.b,
                alpha
            );

            b.rect.anchoredPosition = new Vector2(
                W * 0.5f + b.skew + xShimmer,
                b.yOffset
            );

            bands[i] = b;
        }
    }

    private void UpdateBandTransform(Band b)
    {
        b.rect.anchoredPosition = new Vector2(W * 0.5f + b.skew, b.yOffset);
    }
}
