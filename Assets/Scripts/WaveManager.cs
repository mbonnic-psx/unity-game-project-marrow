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

        enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
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
}
