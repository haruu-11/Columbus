using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Settings")]
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float spawnInterval = 4f;

    private float _nextSpawnTime;

    private void Update()
    {
        if (Time.time >= _nextSpawnTime)
        {
            SpawnEnemy();
            _nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        // Previene un error crítico si el array está vacío en el inspector
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomIndex], hit.position, Quaternion.identity);
        }
    }
}