using UnityEngine;

/// <summary>
/// Binds a spawned enemy to its <see cref="EnemyTypeSO"/> and pushes that type's stats onto its components.
///
/// Two jobs: it tells ObjectPool which queue this instance came from (so a mixed pool can return an enemy to
/// the right place), and it re-applies stats on every respawn — pooled objects keep whatever the last life
/// left behind, so stats have to be pushed on spawn rather than trusted from the prefab.
/// </summary>
[DisallowMultipleComponent]
public class EnemyIdentity : MonoBehaviour
{
    [SerializeField] private EnemyTypeSO type;

    public EnemyTypeSO Type => type;

    /// <summary>Set by ObjectPool when the instance is created, so prefabs don't each need wiring by hand.</summary>
    public void SetType(EnemyTypeSO enemyType)
    {
        type = enemyType;
    }

    public void ApplyStats(EnemyHealth health, EnemyNav nav, EnemyAttack attack)
    {
        if (type == null)
        {
            return;   // untyped enemy — keep whatever the prefab was authored with
        }

        if (health != null)
        {
            health.SetMaxHealth(type.maxHealth);
        }

        if (attack != null)
        {
            attack.SetDamage(type.damage);
        }

        if (nav != null)
        {
            nav.ApplyMovement(type.moveSpeed, type.acceleration, type.angularSpeed);

            if (type.overrideAgentSize)
            {
                nav.ApplyAgentSize(type.agentRadius, type.agentHeight);
            }
        }
    }
}
