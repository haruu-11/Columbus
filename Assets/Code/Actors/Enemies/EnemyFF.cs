using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFF : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float combatDistance = 12f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float repositionCooldown = 3.5f;

    private NavMeshAgent _agent;
    private float _nextFireTime;
    private float _nextRepositionTime;
    private float _sqrCombatDistance;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        _sqrCombatDistance = combatDistance * combatDistance;
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 lookPosition = player.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);

        float sqrDistance = (player.position - transform.position).sqrMagnitude;

        if (sqrDistance > _sqrCombatDistance)
        {
            _agent.SetDestination(player.position);
        }
        else
        {
            if (Time.time >= _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + fireRate;
            }

            if (Time.time >= _nextRepositionTime)
            {
                Reposition();
                _nextRepositionTime = Time.time + repositionCooldown;
            }
        }
    }

    private void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // TryGetComponent evita generar errores si el prefab no tiene el script
            if (projectile.TryGetComponent<EnemyProjectile>(out var projectileScript))
            {
                projectileScript.Initialize(projectileSpeed, 10);
            }
        }
    }

    private void Reposition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 6f;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 6f, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }
}