using System.Collections;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Vector2 spawnIntervalRange = new Vector2(3, 10);

    public EnemyController[] enemyPrefabs;

    private float currentSpawnInterval;

    private void Start()
    {
        StartCoroutine(DoSpawnEnemies());
    }
    
    private IEnumerator DoSpawnEnemies()
    {
        while (true)
        {
            currentSpawnInterval = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
            yield return new WaitForSeconds(currentSpawnInterval);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        EnemyController spawnedEnemy =  Instantiate(GetRandomEnemyPrefab(), transform.position, Quaternion.identity);

        if (GameManager.Instance.Player)
        {
            spawnedEnemy.chaseTarget = GameManager.Instance.Player.transform;
        }
        
    }

    private EnemyController GetRandomEnemyPrefab()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }
}
