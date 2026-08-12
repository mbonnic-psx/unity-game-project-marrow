using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int enemiesAlive = 0;
    [SerializeField] private int baseEnemies = 6;
    [SerializeField] private int enemiesThisWave;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private int maxActiveEnemies = 24;
    private int enemiesRemainingToSpawn;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<Transform> spawnPoints;
    //private bool isWaveEnding = false;

    [Header("Spawn placement (bhop tuning)")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float leadTime = 1.2f;  // secs ahead to extrapolate — raise if you outrun spawns
    [SerializeField] private float minSpawnDist = 8f;    // never pop in on top of the player
    [SerializeField] private float maxSpawnDist = 40f;   // never spawn across the map
    [SerializeField] private float bandFalloff = 15f;   // how forgiving the "where you'll be" band is
    [SerializeField] private float aheadWeight = 1.0f;  // reward points in your travel direction
    [SerializeField] private float bandWeight = 1.5f;  // reward points near your predicted position
    [SerializeField] private float selectionSharpness = 1.5f; // higher = favors best point harder, lower = more even spread
    private readonly List<Transform> _candidates = new List<Transform>();
    private readonly List<float> _weights = new List<float>();

    #region Getters
    public int CurrentWave => currentWave;
    public int EnemiesThisWave => enemiesThisWave;
    public int EnemiesAlive => enemiesAlive;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartWave();
    }

    public void StartWave()
    {
        //isWaveEnding = false;
        currentWave++;
        enemiesThisWave = baseEnemies + (currentWave * 2);
        enemiesAlive = enemiesThisWave;
        enemiesRemainingToSpawn = enemiesThisWave;

        int intialSpawn = Mathf.Min(enemiesThisWave, maxActiveEnemies);
        for (int i = 0; i < intialSpawn; i++)
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        GameObject enemy = objectPool.GetEnemy();

        if (enemiesRemainingToSpawn <= 0)
        {
            return;
        }

        if (enemy == null)
        {
            return;
        }

        //enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        Transform sp = PickSpawnPoint();
        if (sp == null) return;
        enemy.transform.position = sp.position;
        enemy.transform.rotation = Quaternion.identity;
        enemy.SetActive(true);

        EnemyStateMachine esm = enemy.GetComponent<EnemyStateMachine>();
        esm.SetPlayerTransform(playerTransform);
        esm.SetWaveManager(this);
        esm.SetObjectPool(objectPool);
        esm.ResetEnemy();

        enemiesRemainingToSpawn--;


    }

    public void EnemyDied()
    {
        enemiesAlive--;
        if (enemiesRemainingToSpawn > 0)
        {
            SpawnEnemy();
        }

        if (enemiesAlive <= 0)
        {
            Invoke(nameof(StartWave), timeBetweenWaves);
        }
    }

    public void AddSpawnPoints(Transform sp)
    {
        if (!spawnPoints.Contains(sp))
        {
            //Debug.Log($"Adding spawn point: {sp.name} | Active: {sp.gameObject.activeInHierarchy}");
            spawnPoints.Add(sp);
        }
    }

    private Transform PickSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0) return null;
        if (playerMovement == null && playerTransform != null)
            playerMovement = playerTransform.GetComponent<PlayerMovement>();

        Vector3 vel = playerMovement != null ? playerMovement.HorizontalVelocity : Vector3.zero;
        Vector3 dir = vel.sqrMagnitude > 0.5f ? vel.normalized : playerTransform.forward;
        Vector3 predicted = playerTransform.position + vel * leadTime;

        _candidates.Clear();
        _weights.Clear();
        float total = 0f;

        foreach (Transform sp in spawnPoints)
        {
            if (sp == null || !sp.gameObject.activeInHierarchy) continue;

            Vector3 to = sp.position - playerTransform.position;
            float dist = to.magnitude;
            if (dist < minSpawnDist || dist > maxSpawnDist) continue;

            float ahead = Vector3.Dot(dir, to.normalized);
            float band = 1f - Mathf.Clamp01(Vector3.Distance(sp.position, predicted) / bandFalloff);
            float score = ahead * aheadWeight + band * bandWeight;

            float weight = Mathf.Exp(score * selectionSharpness); // strictly positive, so roulette works even for negative scores
            _candidates.Add(sp);
            _weights.Add(weight);
            total += weight;
        }

        if (_candidates.Count == 0)
            return spawnPoints[Random.Range(0, spawnPoints.Count)];

        // weighted-random pick — spreads across forward points instead of dumping on the single best
        float r = Random.value * total;
        for (int i = 0; i < _candidates.Count; i++)
        {
            r -= _weights[i];
            if (r <= 0f) return _candidates[i];
        }
        return _candidates[_candidates.Count - 1];
    }

    public void RelocateEnemy(EnemyStateMachine esm)
    {
        Transform sp = PickSpawnPoint();               // same weighted forward-cone pick as spawning
        if (sp == null) return;
        esm.EnemyNav.WarpTo(sp.position);
        esm.EnemyNav.SetDestination(playerTransform.position);  // re-path immediately
    }
}
