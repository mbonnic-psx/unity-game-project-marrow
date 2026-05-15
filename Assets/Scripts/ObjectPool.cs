using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Queue<GameObject> enemyPool = new Queue<GameObject>();
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int poolSize = 30;
    [SerializeField] private Transform poolParent;

    void Awake()
    {
        for(int i = 0; i < poolSize; i++)
        {
            var enemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, poolParent);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }

    public GameObject GetEnemy()
    {
        if(enemyPool.Count > 0)
        {
            return enemyPool.Dequeue();
        }
        return null;
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
}
