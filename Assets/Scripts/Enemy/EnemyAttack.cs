using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float damageAmount = 33f;
    [SerializeField] private float hitRadius = 2f;
    [SerializeField] private LayerMask playerLayer;

    // A swing must not land twice. AttackState drives damage, but an attack clip can also carry an Animation
    // Event calling Attack() (Test Enemy's did), and any future clip authored that way would double-dip again.
    // This makes the double hit impossible at the source rather than relying on clip setup being correct.
    [SerializeField, Min(0f)] private float minTimeBetweenHits = 0.35f;

    private EnemyStateMachine esm;
    private PlayerStats playerStats;
    private float lastHitTime = float.NegativeInfinity;

    void Awake()
    {
        esm = GetComponent<EnemyStateMachine>();

        if (playerLayer.value == 0)
        {
            Debug.LogError($"{name}: EnemyAttack.playerLayer is unset — OverlapSphere matches nothing, so this enemy can never deal damage.", this);
        }
    }

    public void Attack()
    {
        Attack(hitRadius);
    }

    public void Attack(float radius)
    {
        // Guard on time since the last LANDED hit, not since the last swing — a swing that whiffs shouldn't
        // eat the window and swallow a legitimate follow-up.
        if (Time.time - lastHitTime < minTimeBetweenHits)
        {
            return;
        }

        // Never swing shorter than the caller's reach, whatever the prefab's hitRadius happens to be set to.
        float r = Mathf.Max(hitRadius, radius);

        Collider[] hitbox = Physics.OverlapSphere(transform.position, r, playerLayer);
        foreach (var hitCollider in hitbox)
        {
            // GetComponentInParent, not a transform equality check: the player's collider may sit on a child
            // of the transform the enemy tracks, and an exact match would silently never hit.
            playerStats = hitCollider.GetComponentInParent<PlayerStats>();
            if (playerStats != null)
            {
                lastHitTime = Time.time;
                playerStats.TakeDamage(damageAmount);
                break;
            }
        }
    }

    // Pooled enemies keep stale state; a recycled enemy must be able to swing immediately.
    public void ResetAttack()
    {
        lastHitTime = float.NegativeInfinity;
    }
}
