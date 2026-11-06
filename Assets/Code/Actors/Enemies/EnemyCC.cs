using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCC : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Settings")]
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private int damage = 15;
    [SerializeField] private float attackRate = 1.5f;

    private NavMeshAgent _agent;
    private float _nextAttackTime;
    private Health _playerHealth;
    private float _sqrAttackDistance;

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

        // Cachear la distancia al cuadrado evita realizar raíces cuadradas en cada frame
        _sqrAttackDistance = attackDistance * attackDistance;
    }

    private void Update()
    {
        if (player == null) return;

        float sqrDistance = (player.position - transform.position).sqrMagnitude;

        if (sqrDistance > _sqrAttackDistance)
        {
            _agent.isStopped = false;
            _agent.SetDestination(player.position);
        }
        else
        {
            _agent.isStopped = true;

            // Suaviza la rotación para que el enemigo siga mirando al jugador mientras ataca
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Evitamos que el enemigo se incline hacia arriba o abajo

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            if (Time.time >= _nextAttackTime)
            {
                AttackPlayer();
                _nextAttackTime = Time.time + attackRate;
            }
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