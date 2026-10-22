
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyProximityAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private AudioSource ambientSource;

    [Header("Distance Settings")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField, Range(0f, 1f)] private float minVolume = 0f;
    [SerializeField, Range(0f, 1f)] private float maxVolume = 0.8f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float refreshInterval = 1f;

    private Transform[] enemies;
    private float nextRefreshTime;

    private void Awake()
    {
        if (ambientSource == null)
            ambientSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        RefreshEnemies();
    }

    private void Update()
    {
        if (player == null || ambientSource == null)
            return;

        if (Time.time >= nextRefreshTime)
            RefreshEnemies();

        float nearestDistance = maxDistance;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance = Vector3.Distance(player.position, enemy.position);
            nearestDistance = Mathf.Min(nearestDistance, distance);
        }

        float proximity = 1f - Mathf.Clamp01(nearestDistance / maxDistance);
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, proximity);

        ambientSource.volume = Mathf.MoveTowards(
            ambientSource.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );
    }

    private void RefreshEnemies()
    {
        EnemyFF[] rangedEnemies = FindObjectsByType<EnemyFF>(FindObjectsSortMode.None);
        EnemyCC[] meleeEnemies = FindObjectsByType<EnemyCC>(FindObjectsSortMode.None);

        enemies = new Transform[rangedEnemies.Length + meleeEnemies.Length];

        int index = 0;

        foreach (EnemyFF enemy in rangedEnemies)
            enemies[index++] = enemy.transform;

        foreach (EnemyCC enemy in meleeEnemies)
            enemies[index++] = enemy.transform;

        nextRefreshTime = Time.time + refreshInterval;
    }
}
