using UnityEngine;

public class Health : MonoBehaviour, IResettable
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;

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
        Debug.Log(gameObject.name + " recibió " + amount + " de daño. Vida actual: " + _currentHealth);

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _deathHandler.Die();
        }
    }

    public void ResetState()
    {
        _currentHealth = maxHealth;
        print($"RESET. VIDA: {_currentHealth}");
    }

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
    public float HealthPercentage => maxHealth > 0
    ? (float)_currentHealth / maxHealth
    : 0f;
}