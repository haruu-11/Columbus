using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCC : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private LayerMask obstacleMask; // Acordate de usar la Layer "Walls", no "Ground"

    [Header("Combat Settings")]
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private int damage = 15;
    [SerializeField] private float attackRate = 1.5f;

    private NavMeshAgent _agent;
    private float _nextAttackTime;
    private Health _playerHealth;

    private float _sqrAttackDistance;
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

        if (player != null)
        {
            _playerHealth = player.GetComponent<Health>();
        }

        _sqrAttackDistance = attackDistance * attackDistance;
        _sqrDetectionRadius = detectionRadius * detectionRadius;
    }

    private void Update()
    {
        if (player == null) return;

        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        bool canSeePlayer = (sqrDistance <= _sqrDetectionRadius) && HasLineOfSight();

        if (canSeePlayer)
        {
            _isAggro = true;
        }
        else
        {
            _isAggro = false;
            _agent.isStopped = true;
            return;
        }

        if (sqrDistance > _sqrAttackDistance)
        {
            _agent.isStopped = false;
            _agent.SetDestination(player.position);
        }
        else
        {
            _agent.isStopped = true;
            LookAtPlayer();

            if (Time.time >= _nextAttackTime)
            {
                AttackPlayer();
                _nextAttackTime = Time.time + attackRate;
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

    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void AttackPlayer()
    {
        if (_playerHealth != null)
        {
            _playerHealth.TakeDamage(damage);
        }
    }
}