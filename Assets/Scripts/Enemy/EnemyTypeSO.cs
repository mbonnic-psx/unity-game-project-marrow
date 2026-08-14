using UnityEngine;

/// <summary>
/// Per-enemy-type stats. One asset per roster entry (Skeleton, Sprinter, Brute, Crawler...).
///
/// Everything here used to live as serialized fields spread across the prefab's components and as hardcoded
/// floats inside ChaseState/AttackState, which meant a new enemy type was a code change. Now a type is data:
/// duplicate an asset, retune it, add it to the ObjectPool list.
/// </summary>
[CreateAssetMenu(fileName = "EnemyType", menuName = "Marrow/Enemy Type")]
public class EnemyTypeSO : ScriptableObject
{
    [Header("Identity")]
    public string displayName = "Enemy";
    public GameObject prefab;
    [Min(1)] public int poolSize = 20;

    [Header("Durability & damage")]
    [Min(1f)] public float maxHealth = 100f;
    [Min(0f)] public float damage = 33f;

    [Header("Movement")]
    [Min(0.1f)] public float moveSpeed = 3.5f;
    [Min(0.1f)] public float acceleration = 8f;
    [Min(0f)] public float angularSpeed = 120f;

    [Header("Agent size (crawlers sit low and should fit under things)")]
    public bool overrideAgentSize = false;
    [Min(0.05f)] public float agentRadius = 0.5f;
    [Min(0.1f)] public float agentHeight = 2f;

    [Header("Attack")]
    [Min(0.05f)] public float attackCoolDown = 2f;
    [Min(0f)] public float attackWindup = 0.6f;
    [Min(0.1f)] public float attackRange = 2.5f;
    // Must exceed attackRange: with one shared threshold the enemy flips Chase<->Attack every tick against a
    // moving player and the cooldown restarts before a swing can ever land. OnValidate enforces the gap.
    [Min(0.1f)] public float attackExitRange = 3.2f;

    [Header("Chase / interception")]
    [Min(0f)] public float leadTime = 0.5f;
    [Min(0f)] public float maxLeadDistance = 5f;
    [Min(0f)] public float interceptRange = 4f;

    [Header("Spawning")]
    [Min(0f)] public float spawnWeight = 1f;   // 0 = never spawns
    [Min(1)] public int firstWave = 1;         // type stays out of the mix until this wave

    private void OnValidate()
    {
        // Inspector zeros and inverted ranges are a known landmine in this project — a shared or inverted
        // range here silently produces an enemy that can never land a hit.
        if (attackExitRange < attackRange + 0.5f)
        {
            attackExitRange = attackRange + 0.5f;
        }

        if (attackWindup > attackCoolDown)
        {
            attackWindup = attackCoolDown;
        }

        if (interceptRange < attackExitRange)
        {
            interceptRange = attackExitRange;
        }
    }
}
