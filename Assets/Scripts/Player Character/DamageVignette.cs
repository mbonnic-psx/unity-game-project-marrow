using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Temporary COD-Zombies-style damage read: a red vignette that closes in like tunnel vision as health drops.
/// Intentionally self-contained — it builds its own canvas, image, and gradient texture at runtime, so it
/// needs no art assets and no scene setup beyond dropping the component on the player.
///
/// This is a debug/readability stopgap for Phase 4's "health feedback" task. When that gets built properly,
/// the driving logic (health fraction -> intensity, hit punch) is the part worth keeping; the procedural
/// texture is the part to throw away in favour of a real vignette shader or sprite.
/// </summary>
[DisallowMultipleComponent]
public class DamageVignette : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private int sortingOrder = 100;   // if the death fade-to-black draws under this, lower it

    [Header("Intensity vs. health")]
    // With damageAmount 33 and maxHealth 99: 1 hit = 0.67, 2 hits = 0.33. So full effect at 0.34 means the
    // second hit of three is the moment the screen properly closes in — which is the read we want.
    [SerializeField, Range(0f, 1f)] private float startHealthFraction = 0.99f;
    [SerializeField, Range(0f, 1f)] private float fullEffectHealthFraction = 0.34f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.85f;

    [Header("Hit punch")]
    // Health-driven intensity alone gives no kick on the FIRST hit, so each hit also spikes the vignette
    // briefly. This is the part that reads as "I just got hit" rather than "I am hurt".
    [SerializeField, Range(0f, 1f)] private float punchAmount = 0.55f;
    [SerializeField] private float punchDecay = 2.5f;
    [SerializeField] private float intensityLerpSpeed = 8f;

    [Header("Tunnel vision")]
    // Oversizing the image pushes the gradient's opaque ring off-screen; shrinking it toward screen size
    // walks that ring inward. That is the whole tunnel effect — no shader, no per-frame texture work.
    [SerializeField] private float overscanHealthy = 1200f;
    [SerializeField] private float overscanHurt = 0f;

    [Header("Look")]
    [SerializeField] private Color vignetteColor = new Color(0.6f, 0f, 0f, 1f);
    [SerializeField, Range(0f, 1f)] private float innerRadius = 0.30f;   // where the red starts bleeding in
    [SerializeField, Range(0f, 2f)] private float outerRadius = 1.05f;   // where it reaches full opacity
    [SerializeField] private int textureSize = 256;

    [Header("Debug")]
    [SerializeField] private KeyCode testHitKey = KeyCode.H;   // fake a hit without needing an enemy; set to None to disable

    private Image image;
    private RectTransform imageRect;
    private Texture2D gradient;
    private float punch;
    private float currentIntensity;
    private float healthFraction = 1f;

    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = GetComponentInParent<PlayerStats>();
        }

        if (playerStats == null)
        {
            Debug.LogError($"{name}: DamageVignette found no PlayerStats — the vignette will never react to damage.", this);
            enabled = false;
            return;
        }

        BuildOverlay();
    }

    private void OnEnable()
    {
        if (playerStats == null) return;
        playerStats.OnDamaged += HandleDamaged;
        playerStats.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        if (playerStats == null) return;
        playerStats.OnDamaged -= HandleDamaged;
        playerStats.OnHealthChanged -= HandleHealthChanged;
    }

    private void Start()
    {
        // Read once after PlayerStats.Awake has set currentHealth, so a mid-run enable doesn't start at full.
        HandleHealthChanged(playerStats.CurrentHealth, playerStats.PlayerHealth);
        currentIntensity = TargetIntensity();
        Apply(currentIntensity);
    }

    private void Update()
    {
        if (testHitKey != KeyCode.None && Input.GetKeyDown(testHitKey))
        {
            HandleDamaged(0f);
        }

        // Unscaled: hitstop is a planned Phase 6 feature, and the damage read should keep animating through it.
        float dt = Time.unscaledDeltaTime;

        punch = Mathf.Max(0f, punch - punchDecay * dt);

        float target = TargetIntensity();
        currentIntensity = Mathf.Lerp(currentIntensity, target, 1f - Mathf.Exp(-intensityLerpSpeed * dt));

        Apply(Mathf.Clamp01(currentIntensity + punch));
    }

    private float TargetIntensity()
    {
        // Inverted range: health FALLING from startHealthFraction to fullEffectHealthFraction drives 0 -> 1.
        float t = Mathf.InverseLerp(startHealthFraction, fullEffectHealthFraction, healthFraction);
        return Mathf.Clamp01(t);
    }

    private void Apply(float intensity)
    {
        if (image == null) return;

        Color c = vignetteColor;
        c.a = intensity * maxAlpha;
        image.color = c;

        float overscan = Mathf.Lerp(overscanHealthy, overscanHurt, intensity);
        imageRect.sizeDelta = new Vector2(overscan, overscan);
    }

    private void HandleDamaged(float amount)
    {
        punch = punchAmount;
    }

    private void HandleHealthChanged(float current, float max)
    {
        healthFraction = max > 0f ? Mathf.Clamp01(current / max) : 0f;
    }

    private void BuildOverlay()
    {
        gradient = BuildRadialGradient();

        var canvasGo = new GameObject("DamageVignetteCanvas");
        canvasGo.transform.SetParent(transform, false);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        var imageGo = new GameObject("Vignette");
        imageGo.transform.SetParent(canvasGo.transform, false);

        image = imageGo.AddComponent<Image>();
        image.sprite = Sprite.Create(gradient, new Rect(0f, 0f, gradient.width, gradient.height), new Vector2(0.5f, 0.5f));
        image.type = Image.Type.Simple;
        image.raycastTarget = false;   // a full-screen overlay would otherwise swallow every UI click

        imageRect = image.rectTransform;
        imageRect.anchorMin = Vector2.zero;      // stretch to screen; sizeDelta then reads as "extra pixels beyond it"
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;
        imageRect.sizeDelta = new Vector2(overscanHealthy, overscanHealthy);

        Apply(0f);
    }

    private Texture2D BuildRadialGradient()
    {
        int size = Mathf.Max(32, textureSize);
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,   // stops the opaque edge tiling back across the screen
            filterMode = FilterMode.Bilinear
        };

        var pixels = new Color32[size * size];
        float half = size * 0.5f;
        byte r = (byte)(vignetteColor.r * 255f);
        byte g = (byte)(vignetteColor.g * 255f);
        byte b = (byte)(vignetteColor.b * 255f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x + 0.5f - half) / half;
                float dy = (y + 0.5f - half) / half;
                float d = Mathf.Sqrt(dx * dx + dy * dy);

                float t = Mathf.InverseLerp(innerRadius, outerRadius, d);
                t = t * t * (3f - 2f * t);   // smoothstep — a linear ramp reads as a hard band

                pixels[y * size + x] = new Color32(r, g, b, (byte)(Mathf.Clamp01(t) * 255f));
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        return tex;
    }
}
