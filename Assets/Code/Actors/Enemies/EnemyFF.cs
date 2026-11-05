using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFF : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Audio SFX (Bloque C)")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip alertClip;       // NUEVO: Sonido cuando te detecta
    [SerializeField] private AudioClip escupitajoClip;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Combat Settings")]
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float combatDistance = 12f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float repositionCooldown = 3.5f;

    private NavMeshAgent _agent;
    private float _nextFireTime;
    private float _nextRepositionTime;
    private float _sqrCombatDistance;
    private float _sqrDetectionRadius;
    private bool _isAggro = false;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        _sqrCombatDistance = combatDistance * combatDistance;
        _sqrDetectionRadius = detectionRadius * detectionRadius;
    }

    private void Update()
    {
        if (player == null) return;

        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        bool canSeePlayer = (sqrDistance <= _sqrDetectionRadius) && HasLineOfSight();

        if (canSeePlayer)
        {
            // Si recién te detecta, grita.
            if (!_isAggro)
            {
                _isAggro = true;
                if (sfxSource != null && alertClip != null)
                {
                    sfxSource.PlayOneShot(alertClip);
                }
            }
        }
        else
        {
            _isAggro = false;
            _agent.isStopped = true;
            return;
        }

        Vector3 lookPosition = player.position;
        lookPosition.y = transform.position.y;
        transform.LookAt(lookPosition);

        if (sqrDistance > _sqrCombatDistance)
        {
            _agent.isStopped = false;
            _agent.SetDestination(player.position);
        }
        else
        {
            _agent.isStopped = true;

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

    private bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 directionToPlayer = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        if (Physics.Raycast(origin, directionToPlayer, distance, obstacleMask))
        {
            return false;
        }
        return true;
    }

    private void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            if (projectile.TryGetComponent<EnemyProjectile>(out var projectileScript))
            {
                projectileScript.Initialize(projectileSpeed, 10);
            }

            if (sfxSource != null && escupitajoClip != null)
            {
                sfxSource.pitch = Random.Range(0.85f, 1.15f);
                sfxSource.volume = Random.Range(0.8f, 1.0f);
                sfxSource.PlayOneShot(escupitajoClip);
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