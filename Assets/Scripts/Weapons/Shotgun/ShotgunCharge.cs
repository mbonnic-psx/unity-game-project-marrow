using UnityEngine;
using UnityEngine.Events;

/* <summary>
    Tracks kills and converts them into shotgun shells (charges).
    Completely decoupled from the weapon — any kill source (throwables, the
    shotgun itself, environmental kills) can call RegisterKill().

    Made with Claude Opus 4.8 - High
*/

public class ShotgunCharge : MonoBehaviour
{
    [Header("Charge Economy")]
    [Tooltip("Kills needed per shell. MUST be at least 1 — setting this to 0 " +
            "does NOT give infinite ammo, it just means one kill = one shell. " +
            "Use 'Infinite Ammo' under DEBUG for that.")]
    [Min(1)]
    [SerializeField] int killsPerCharge = 5;

    [Min(1)]
    [SerializeField] int maxCharges = 3;

    [Tooltip("If true, kills earned while at max shells still bank progress " +
             "toward the next one. If false, they're wasted — punishes hoarding.")]
    [SerializeField] bool bankProgressWhenFull = false;

    // ---------------------------------------------------------------- DEBUG
    [Header("--- DEBUG ---")]
    [Tooltip("Fire forever without spending shells. THIS is the 'always fire' switch.")]
    [SerializeField] bool infiniteAmmo = false;

    [Tooltip("Shells you spawn with. Set to 3 to skip the grind while tuning feel.")]
    [SerializeField] int startingCharges = 0;

    [SerializeField] bool logCharges = true;
    // -----------------------------------------------------------------------

    [Header("Events")]
    public UnityEvent<int, int> OnChargesChanged;   // (current, max)
    public UnityEvent<float> OnProgressChanged;  // 0–1 toward next shell
    public UnityEvent OnChargeGained;     // hook: shell pickup stinger
    public UnityEvent OnChargeSpent;      // hook: shell eject sfx
    public UnityEvent OnFireFailed;       // hook: dry click

    int charges;
    int killsTowardNext;

    public int Charges => charges;
    public int MaxCharges => maxCharges;
    public bool HasCharge => infiniteAmmo || charges > 0;
    public bool InfiniteAmmo => infiniteAmmo;

    void Awake()
    {
        // Belt and braces — [Min(1)] guards the inspector, this guards prefabs
        // and older serialized values that were saved as 0.
        if (killsPerCharge < 1)
        {
            Debug.LogWarning($"[ShotgunCharge] killsPerCharge was {killsPerCharge}, which causes a " +
                             "divide-by-zero (NaN) in the progress event. Clamping to 1. " +
                             "If you wanted to always fire, tick 'Infinite Ammo' instead.", this);
            killsPerCharge = 1;
        }

        charges = Mathf.Clamp(startingCharges, 0, maxCharges);
    }

    void Start()
    {
        if (logCharges)
        {
            Debug.Log($"[ShotgunCharge] Ready. Shells {charges}/{maxCharges} | " +
                      $"{killsPerCharge} kills per shell | Infinite ammo: {infiniteAmmo}");

            if (!infiniteAmmo && charges == 0)
                Debug.Log("[ShotgunCharge] Starting with 0 shells. Until something calls RegisterKill(), " +
                          "every shot will DRY CLICK. Wire this into EnemyHealth's death, or set " +
                          "'Starting Charges' / 'Infinite Ammo' to test.");
        }

        Broadcast();
    }

    /// <summary>Call this from EnemyHealth when an enemy dies.</summary>
    public void RegisterKill()
    {
        bool atMax = charges >= maxCharges;

        if (atMax && !bankProgressWhenFull)
        {
            OnProgressChanged?.Invoke(1f);
            return;
        }

        killsTowardNext++;

        if (killsTowardNext >= killsPerCharge && charges < maxCharges)
        {
            killsTowardNext = 0;
            charges++;
            OnChargeGained?.Invoke();

            if (logCharges)
                Debug.Log($"[ShotgunCharge] Shell gained. {charges}/{maxCharges}");
        }

        Broadcast();
    }

    /// <summary>Spends one shell. Returns false if empty (and fires OnFireFailed).</summary>
    public bool TryConsumeCharge()
    {
        if (infiniteAmmo)
        {
            OnChargeSpent?.Invoke();
            return true;
        }

        if (charges <= 0)
        {
            OnFireFailed?.Invoke();
            return false;
        }

        charges--;
        OnChargeSpent?.Invoke();

        // If we were sitting at max with banked progress, it may complete a
        // shell immediately now that a slot has opened up.
        if (bankProgressWhenFull && killsTowardNext >= killsPerCharge && charges < maxCharges)
        {
            killsTowardNext = 0;
            charges++;
            OnChargeGained?.Invoke();
        }

        if (logCharges)
            Debug.Log($"[ShotgunCharge] Shell spent. {charges}/{maxCharges}");

        Broadcast();
        return true;
    }

    /// <summary>DEBUG: instantly grant a shell without any kills.</summary>
    public void GrantCharge()
    {
        if (charges >= maxCharges) return;

        charges++;
        killsTowardNext = 0;
        OnChargeGained?.Invoke();
        Broadcast();

        Debug.Log($"[ShotgunCharge DEBUG] Shell granted. {charges}/{maxCharges}");
    }

    /// <summary>DEBUG: toggle infinite ammo at runtime.</summary>
    public void ToggleInfiniteAmmo()
    {
        infiniteAmmo = !infiniteAmmo;
        Debug.Log($"[ShotgunCharge DEBUG] Infinite ammo: {infiniteAmmo}");
        Broadcast();
    }

    void Broadcast()
    {
        OnChargesChanged?.Invoke(charges, maxCharges);

        // Guarded — killsPerCharge is clamped in Awake, but never trust it.
        float progress = killsPerCharge > 0
            ? (float)killsTowardNext / killsPerCharge
            : 1f;

        OnProgressChanged?.Invoke(Mathf.Clamp01(progress));
    }
}
