using UnityEngine;

/// <summary>
/// The wind-up tell before an enemy swing: the enemy tints and swells over the wind-up window, with an
/// optional audio cue at the start of it.
///
/// AttackState already holds a real wind-up window (attackWindup) between entering attack range and the
/// swing actually landing — this is the presentation for that window, so damage stops arriving unannounced.
/// Optional component: enemies without it behave exactly as before.
/// </summary>
[DisallowMultipleComponent]
public class EnemyTelegraph : MonoBehaviour
{
    [Header("Tint")]
    [SerializeField] private Color telegraphColor = new Color(1f, 0.25f, 0.15f, 1f);
    [SerializeField, Range(0f, 1f)] private float maxTint = 0.85f;

    [Header("Swell")]
    // Left null, this auto-targets the model rather than the root: the root carries the NavMeshAgent and
    // collider, and scaling those would change the enemy's actual hitbox and steering, not just its look.
    [SerializeField] private Transform scaleRoot;
    [SerializeField] private float scalePunch = 0.12f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip windupClip;

    [Header("Timing")]
    [SerializeField] private float recoverySpeed = 5f;   // how fast the tell drops off once the swing has landed

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");   // URP lit/unlit
    private static readonly int ColorId = Shader.PropertyToID("_Color");           // sprite / built-in

    private Renderer[] renderers;
    private MaterialPropertyBlock mpb;
    private Color[] baseColors;
    private int[] colorPropIds;   // 0 = this renderer has no colour property worth driving
    private Vector3 baseScale;
    private float windupDuration;
    private float windupTimer;
    private bool winding;
    private float tell;           // 0 = normal, 1 = fully telegraphed

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        CacheRenderers();

        if (scaleRoot == null && renderers.Length > 0 && renderers[0].transform != transform)
        {
            scaleRoot = renderers[0].transform;
        }

        if (scaleRoot != null)
        {
            baseScale = scaleRoot.localScale;
        }
    }

    private void CacheRenderers()
    {
        // MaterialPropertyBlock rather than renderer.material: touching .material instantiates a copy per
        // enemy, which leaks across a pooled roster and breaks batching.
        renderers = GetComponentsInChildren<Renderer>(true);
        mpb = new MaterialPropertyBlock();
        baseColors = new Color[renderers.Length];
        colorPropIds = new int[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].sharedMaterial;
            if (mat == null)
            {
                continue;
            }

            if (mat.HasProperty(BaseColorId))
            {
                colorPropIds[i] = BaseColorId;
                baseColors[i] = mat.GetColor(BaseColorId);
            }
            else if (mat.HasProperty(ColorId))
            {
                colorPropIds[i] = ColorId;
                baseColors[i] = mat.GetColor(ColorId);
            }
        }
    }

    /// <summary>Called by AttackState the moment the wind-up starts, i.e. `duration` seconds before the hit.</summary>
    public void BeginWindup(float duration)
    {
        windupDuration = Mathf.Max(0f, duration);
        windupTimer = 0f;
        winding = true;

        if (audioSource && windupClip) audioSource.PlayOneShot(windupClip);
    }

    private void Update()
    {
        if (winding)
        {
            windupTimer += Time.deltaTime;
            float t = windupDuration > 0f ? Mathf.Clamp01(windupTimer / windupDuration) : 1f;

            // Squared, not linear: a linear ramp is barely perceptible early and reads as the enemy simply
            // being red. Accelerating into the swing is what makes it read as "about to hit you".
            tell = t * t;

            if (windupTimer >= windupDuration)
            {
                winding = false;   // swing lands here; the tell then falls off on its own
            }
        }
        else if (tell > 0f)
        {
            tell = Mathf.MoveTowards(tell, 0f, recoverySpeed * Time.deltaTime);
        }
        else
        {
            return;   // idle: nothing to push to the renderers
        }

        ApplyTell();
    }

    private void ApplyTell()
    {
        float amount = tell * maxTint;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (colorPropIds[i] == 0 || renderers[i] == null)
            {
                continue;
            }

            renderers[i].GetPropertyBlock(mpb);
            mpb.SetColor(colorPropIds[i], Color.Lerp(baseColors[i], telegraphColor, amount));
            renderers[i].SetPropertyBlock(mpb);
        }

        if (scaleRoot != null)
        {
            scaleRoot.localScale = baseScale * (1f + scalePunch * tell);
        }
    }

    /// <summary>Pooled enemies must not respawn mid-tell, still red and still swollen.</summary>
    public void ResetTelegraph()
    {
        winding = false;
        windupTimer = 0f;
        tell = 0f;
        ApplyTell();
    }
}
