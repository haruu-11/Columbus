using UnityEngine;

public class Health : MonoBehaviour, IResettable
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem bloodParticles;

    private int _currentHealth;
    private IDeathHandler _deathHandler;

    private void Start()
    {
        _currentHealth = maxHealth;

        if (!TryGetComponent(out _deathHandler))
        {
            Debug.LogError($"Objeto con {nameof(Health)} necesita también un script que implemente {nameof(IDeathHandler)}");
        }
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;

        if (bloodParticles != null)
        {
            bloodParticles.Play();
        }

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;

            // Solo desvinculamos y destruimos las partículas si el objeto NO es el jugador.
            if (!gameObject.CompareTag("Player") && bloodParticles != null)
            {
                bloodParticles.transform.SetParent(null);
                // Destruimos el objeto de partículas tras 2 segundos para darle tiempo a la animación.
                Destroy(bloodParticles.gameObject, 2f);
            }

            _deathHandler.Die();
        }
    }

    public void ResetState()
    {
        _currentHealth = maxHealth;

        // Al respawnear, detenemos y limpiamos las partículas emitidas para que no viajen al inicio.
        if (bloodParticles != null)
        {
            bloodParticles.Stop();
            bloodParticles.Clear();
        }
    }

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
    public float HealthPercentage => maxHealth > 0 ? (float)_currentHealth / maxHealth : 0f;
}