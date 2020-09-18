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
            GameManager.Instance.SpawnEnemy(GetRandomEnemyPrefab(), transform.position);
        }
    }

    private EnemyController GetRandomEnemyPrefab()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }
}
