
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCC : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    [Header("Audio SFX (Bloque C)")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip alertClip;
    [SerializeField] private AudioClip attackClip;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private LayerMask obstacleMask;

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

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>(true);
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
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
            UpdateMovementAnimation(false);
            return;
        }

        if (sqrDistance > _sqrAttackDistance)
        {
            _agent.isStopped = false;
            _agent.SetDestination(player.position);
            UpdateMovementAnimation(true);
        }
        else
        {
            _agent.isStopped = true;
            UpdateMovementAnimation(false);
            LookAtPlayer();

            if (Time.time >= _nextAttackTime)
            {
                AttackPlayer();
                _nextAttackTime = Time.time + attackRate;
            }
        }
    }

    private void UpdateMovementAnimation(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
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
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }
    }

    private void AttackPlayer()
    {
        // Activar la animación de ataque cada vez que ataca.
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        // Sonido de ataque con variación de pitch.
        if (sfxSource != null && attackClip != null)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.15f);
            sfxSource.PlayOneShot(attackClip);
        }

        if (_playerHealth != null)
        {
            _playerHealth.TakeDamage(damage);
        }
    }
}
