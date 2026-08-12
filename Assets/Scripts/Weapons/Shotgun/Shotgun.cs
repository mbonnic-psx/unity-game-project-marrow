using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/* <summary>
    The wonder weapon. Spends one shell per shot: fires a cone of pellets and
    launches the player in the opposite direction. The shell economy IS the
    movement resource — every shot is a choice between killing and escaping.

    Made with Claude Opus 4.8 - High
*/

public class Shotgun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ShotgunCharge charge;
    [SerializeField] Camera aimCamera;   // the player camera
    [SerializeField] PlayerImpulse impulse;     // on the player root
    [SerializeField] Transform muzzle;      // vfx origin (visual only)
    [SerializeField] KeyCode fireKey;

    [Header("Pellets")]
    [SerializeField] int pellets = 10;
    [SerializeField] float spreadAngle = 8f;   // cone half-angle, degrees
    [SerializeField] float range = 25f;
    [SerializeField] float damagePerPellet = 25f;
    [SerializeField] float propKnockback = 4f;
    [SerializeField] LayerMask hitMask = ~0;

    [Header("Self-Boost")]
    [SerializeField] float boostForce = 14f;
    [SerializeField] float upwardBias = 0.25f;
    [SerializeField] bool cancelPreviousBoost = true;

    [Header("Feel")]
    [SerializeField] float fireCooldown = 0.35f;

    [Header("Feedback")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip fireClip;
    [SerializeField] AudioClip dryClip;

    // ---------------------------------------------------------------- DEBUG
    [Header("--- DEBUG ---")]
    [Tooltip("Master switch for all debug output below.")]
    [SerializeField] bool debugMode = true;

    [Tooltip("Log every stage of the shot to the console.")]
    [SerializeField] bool logShots = true;

    [Tooltip("Draw pellet rays in the SCENE view (green = hit, red = miss). Needs Gizmos on.")]
    [SerializeField] bool drawSceneRays = true;
    [SerializeField] float rayDrawDuration = 2f;

    [Tooltip("Spawn visible tracer lines in the GAME view. This is the one you " +
             "actually want — raycasts are invisible otherwise.")]
    [SerializeField] bool spawnTracers = true;
    [SerializeField] Color tracerColor = new Color(1f, 0.85f, 0.4f);
    [SerializeField] float tracerWidth = 0.02f;
    [SerializeField] float tracerLifetime = 0.06f;

    [Tooltip("Cheat keys: K = fake a kill, C = grant a full shell, F = force fire (spends nothing).")]
    [SerializeField] bool cheatKeys = true;
    // -----------------------------------------------------------------------

    public UnityEvent OnFired;

    float nextFireTime;
    Material tracerMat;

    void Awake()
    {
        if (!charge) charge = FindFirstObjectByType<ShotgunCharge>();
        ValidateReferences();
        if (spawnTracers)
            tracerMat = new Material(Shader.Find("Sprites/Default"));
    }

    void ValidateReferences()
    {
        if (!charge) Debug.LogError("[Shotgun] ShotgunCharge is NOT assigned. You can never fire.", this);
        if (!aimCamera) Debug.LogError("[Shotgun] aimCamera is NOT assigned. Pellets and boost direction will fail.", this);
        if (!impulse) Debug.LogError("[Shotgun] PlayerImpulse is NOT assigned. You will get NO self-boost.", this);
        if (!muzzleFlash) Debug.LogWarning("[Shotgun] muzzleFlash not assigned — THIS is why you see no particles.", this);
        if (!audioSource) Debug.LogWarning("[Shotgun] audioSource not assigned — no sound will play.", this);

        if (hitMask == 0)
            Debug.LogWarning("[Shotgun] hitMask is set to Nothing. Pellets will hit absolutely nothing.", this);

        int playerLayer = gameObject.layer;
        if ((hitMask.value & (1 << playerLayer)) != 0)
            Debug.LogWarning($"[Shotgun] hitMask INCLUDES your own layer ('{LayerMask.LayerToName(playerLayer)}'). " +
                             "Pellets may hit you point-blank. Exclude the player layer.", this);
    }

    void Update()
    {
        if (Input.GetKeyDown(fireKey) && Time.time >= nextFireTime)
            TryFire();

        if (debugMode && cheatKeys)
            HandleCheatKeys();
    }

    void HandleCheatKeys()
    {
        if (!charge) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            charge.RegisterKill();
            Debug.Log($"[Shotgun DEBUG] Faked a kill. Shells: {charge.Charges}/{charge.MaxCharges}");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            int before = charge.Charges;
            for (int i = 0; i < 50; i++)
            {
                charge.RegisterKill();
                if (charge.Charges > before) break;
            }
            Debug.Log($"[Shotgun DEBUG] Granted a shell. Shells: {charge.Charges}/{charge.MaxCharges}");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("[Shotgun DEBUG] FORCE FIRE — bypassing the shell check entirely.");
            ExecuteShot();
        }
    }

    public void TryFire()
    {
        if (!charge)
        {
            Debug.LogError("[Shotgun] Can't fire: ShotgunCharge reference is null.", this);
            return;
        }

        if (!charge.TryConsumeCharge())
        {
            if (debugMode && logShots)
                Debug.Log("[Shotgun] DRY CLICK — 0 shells. TryFire() returns early here, which is why " +
                          "you get no pellets, no particles and no boost. Kill 5 enemies, or press C.");

            if (audioSource && dryClip) audioSource.PlayOneShot(dryClip);
            return;
        }

        nextFireTime = Time.time + fireCooldown;
        ExecuteShot();
    }

    /// <summary>Everything that happens on a successful shot. Split out so the
    /// force-fire debug key can call it without spending a shell.</summary>
    void ExecuteShot()
    {
        if (debugMode && logShots)
            Debug.Log($"[Shotgun] FIRING. Shells left: {(charge ? charge.Charges : -1)}");

        FirePellets();
        ApplySelfBoost();

        if (muzzleFlash) muzzleFlash.Play();
        else if (debugMode && logShots) Debug.LogWarning("[Shotgun] No muzzleFlash assigned — nothing to play.");

        if (audioSource && fireClip) audioSource.PlayOneShot(fireClip);
        OnFired?.Invoke();
    }

    void FirePellets()
    {
        if (!aimCamera) { Debug.LogError("[Shotgun] No aimCamera — skipping pellets.", this); return; }

        Vector3 origin = aimCamera.transform.position;
        Vector3 forward = aimCamera.transform.forward;
        Vector3 tracerOrigin = muzzle ? muzzle.position : origin;

        int hits = 0;
        int enemiesHit = 0;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 dir = RandomConeDirection(forward, spreadAngle);
            bool hitSomething = Physics.Raycast(origin, dir, out RaycastHit hit, range, hitMask);
            Vector3 endPoint = hitSomething ? hit.point : origin + dir * range;

            if (hitSomething)
            {
                hits++;

                if (hit.collider.TryGetComponent(out EnemyHealth enemy))
                {
                    // NOTE: match this to your actual EnemyHealth signature.
                    enemy.TakeDamage(damagePerPellet);
                    enemiesHit++;
                }

                if (hit.rigidbody && !hit.rigidbody.isKinematic)
                    hit.rigidbody.AddForceAtPosition(dir * propKnockback, hit.point, ForceMode.Impulse);
            }

            if (debugMode && drawSceneRays)
                Debug.DrawLine(origin, endPoint, hitSomething ? Color.green : Color.red, rayDrawDuration);

            if (debugMode && spawnTracers)
                StartCoroutine(SpawnTracer(tracerOrigin, endPoint));
        }

        if (debugMode && logShots)
            Debug.Log($"[Shotgun] Pellets: {pellets} fired | {hits} hit geometry | {enemiesHit} hit an EnemyHealth. "
                      + (hits == 0 ? "ZERO hits — check hitMask and range." : ""));
    }

    /// <summary>Uniform cone spread around an axis. (The previous version's
    /// Slerp line was a no-op and the cone was lopsided — this replaces it.)</summary>
    Vector3 RandomConeDirection(Vector3 forward, float halfAngleDegrees)
    {
        Vector2 disc = Random.insideUnitCircle;

        Vector3 axis = Vector3.Cross(forward, Vector3.up);
        if (axis.sqrMagnitude < 0.001f) axis = Vector3.Cross(forward, Vector3.right); // looking straight up/down
        axis.Normalize();

        Quaternion tilt = Quaternion.AngleAxis(disc.magnitude * halfAngleDegrees, axis);
        Quaternion roll = Quaternion.AngleAxis(Mathf.Atan2(disc.y, disc.x) * Mathf.Rad2Deg, forward);

        return (roll * tilt * forward).normalized;
    }

    void ApplySelfBoost()
    {
        if (!impulse)
        {
            Debug.LogError("[Shotgun] No PlayerImpulse assigned — NO BOOST. This is your missing impulse.", this);
            return;
        }
        if (!aimCamera) return;

        Vector3 dir = -aimCamera.transform.forward;
        dir = (dir + Vector3.up * upwardBias).normalized;
        Vector3 launch = dir * boostForce;

        impulse.AddImpulse(launch, cancelPreviousBoost);

        if (debugMode && logShots)
            Debug.Log($"[Shotgun] BOOST: dir {dir} | force {boostForce} | launch {launch} | " +
                      $"PlayerImpulse velocity is now {impulse.Velocity}. " +
                      "If that number is non-zero but you don't MOVE, your movement script is overwriting it.");
    }

    IEnumerator SpawnTracer(Vector3 from, Vector3 to)
    {
        GameObject go = new GameObject("PelletTracer");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = tracerMat;
        lr.startColor = tracerColor;
        lr.endColor = tracerColor;
        lr.startWidth = tracerWidth;
        lr.endWidth = tracerWidth;
        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);

        yield return new WaitForSeconds(tracerLifetime);
        Destroy(go);
    }
}
