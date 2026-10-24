using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private int damage = 10;

    private float _projectileSpeed;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(_projectileSpeed * Time.deltaTime * Vector3.forward);
    }

    public void Initialize(float projectileSpeed, int projectileDamage)
    {
        _projectileSpeed = projectileSpeed;
        damage = projectileDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Resuelve el problema de pedirle al collider su propio collider
        if (other.TryGetComponent<Health>(out var health) || other.GetComponentInParent<Health>())
        {
            Health targetHealth = health != null ? health : other.GetComponentInParent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}