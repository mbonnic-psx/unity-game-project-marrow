using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One pooled queue per enemy type. Was a single Queue against a single prefab, which made a mixed roster
/// impossible — a returned enemy had nowhere type-specific to go back to.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private List<EnemyTypeSO> enemyTypes = new List<EnemyTypeSO>();
    [SerializeField] private Transform poolParent;

    private readonly Dictionary<EnemyTypeSO, Queue<GameObject>> pools = new Dictionary<EnemyTypeSO, Queue<GameObject>>();

    public IReadOnlyList<EnemyTypeSO> EnemyTypes => enemyTypes;

    void Awake()
    {
        foreach (EnemyTypeSO type in enemyTypes)
        {
            if (type == null)
            {
                continue;
            }

            if (type.prefab == null)
            {
                Debug.LogError($"ObjectPool: enemy type '{type.name}' has no prefab — it will never spawn.", this);
                continue;
            }

            if (pools.ContainsKey(type))
            {
                Debug.LogWarning($"ObjectPool: enemy type '{type.name}' is listed twice; ignoring the duplicate.", this);
                continue;
            }

            var queue = new Queue<GameObject>(type.poolSize);
            for (int i = 0; i < type.poolSize; i++)
            {
                GameObject enemy = Instantiate(type.prefab, Vector3.zero, Quaternion.identity, poolParent);

                // Stamped here rather than authored per prefab: it's what lets ReturnEnemy find the right
                // queue, so it must be true for every instance regardless of how the prefab was set up.
                EnemyIdentity identity = enemy.GetComponent<EnemyIdentity>();
                if (identity == null)
                {
                    identity = enemy.AddComponent<EnemyIdentity>();
                }
                identity.SetType(type);

                enemy.SetActive(false);
                queue.Enqueue(enemy);
            }

            pools[type] = queue;
        }

        if (pools.Count == 0)
        {
            Debug.LogError("ObjectPool: no enemy types configured — nothing can spawn. Assign EnemyTypeSO assets on this component.", this);
        }
    }

    public GameObject GetEnemy(EnemyTypeSO type)
    {
        if (type == null || !pools.TryGetValue(type, out Queue<GameObject> queue))
        {
            return null;
        }

        return queue.Count > 0 ? queue.Dequeue() : null;
    }

    /// <summary>Lets the caller check availability before committing, so a spawn can pick a different type
    /// instead of dequeuing from an exhausted pool and bailing out.</summary>
    public bool HasAvailable(EnemyTypeSO type)
    {
        return type != null && pools.TryGetValue(type, out Queue<GameObject> queue) && queue.Count > 0;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.SetActive(false);

        EnemyIdentity identity = enemy.GetComponent<EnemyIdentity>();
        if (identity != null && identity.Type != null && pools.TryGetValue(identity.Type, out Queue<GameObject> queue))
        {
            queue.Enqueue(enemy);
            return;
        }

        // Silently dropping it would shrink the pool every wave until spawning quietly stops.
        Debug.LogWarning($"ObjectPool: '{enemy.name}' returned without a known type — it is leaking out of the pool.", enemy);
    }
}
