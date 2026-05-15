using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThrowableScatter : MonoBehaviour
{
    [Header("Throwable Prefabs")]
    [SerializeField] private List<GameObject> throwablePrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private int spawnedPrefabs = 60;
    [SerializeField] private float searchRadius = 200f;
    [SerializeField] private float minDistanceBetween = 2f;
    [SerializeField] private GameObject prefabParent;

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        ScatterThrowables();
    }

    void ScatterThrowables()
    {
        int spawned = 0;
        int attempts = 0;
        int maxAttempts = spawnedPrefabs * 10;

        while (spawned < spawnedPrefabs && attempts < maxAttempts)
        {
            attempts++;

            // Pick a random point within the search radius
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * searchRadius;
            randomPoint.y = transform.position.y; // keep it at ground level before sampling

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
            {
                // Check it's not too close to another spawned object
                if (IsTooClose(hit.position)) continue;

                GameObject obj = Instantiate(throwablePrefabs[Random.Range(0, throwablePrefabs.Count)], hit.position, Random.rotation);
                if (prefabParent != null)
                    obj.transform.SetParent(prefabParent.transform);

                spawnedPositions.Add(hit.position);
                spawned++;
            }
        }

        Debug.Log($"[ThrowableScatter] Spawned {spawned} throwables.");
    }

    bool IsTooClose(Vector3 pos)
    {
        foreach (Vector3 existing in spawnedPositions)
        {
            if (Vector3.Distance(pos, existing) < minDistanceBetween)
                return true;
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (searchRadius == 0) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
