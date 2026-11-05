using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Settings")]
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float spawnInterval = 4f;

    [Header("Room Limits")]
    [Tooltip("Activar para oleadas infinitas (ideal para el mini survival del final)")]
    [SerializeField] private bool isInfiniteSpawner = false;
    [SerializeField] private int maxEnemiesToSpawn = 3;

    private float _nextSpawnTime;
    private int _spawnedCount = 0;

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
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomIndex], hit.position, Quaternion.identity);

            // Si no es infinito, sumamos a la cuenta y revisamos el límite
            if (!isInfiniteSpawner)
            {
                _spawnedCount++;

                if (_spawnedCount >= maxEnemiesToSpawn)
                {
                    Debug.Log($"Spawner de la sala {gameObject.name} terminó su cupo y se apagó.");
                    this.enabled = false; // Apaga el script. No consume más recursos.
                }
            }
        }
    }
}