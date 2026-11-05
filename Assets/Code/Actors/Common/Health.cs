using UnityEngine;

public class Health : MonoBehaviour, IResettable
{
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem bloodParticles;

    [Header("Audio SFX (Bloque C)")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip[] damageClips;
    [SerializeField] private AudioClip deathClip;

    private int _currentHealth;
    private IDeathHandler _deathHandler;

    // NUEVO: Candado para evitar que muera dos veces en el mismo frame
    private bool _isDead = false;

    private void Start()
    {
        _currentHealth = maxHealth;
        _isDead = false;

        if (!TryGetComponent(out _deathHandler))
        {
            Debug.LogError($"Objeto con {nameof(Health)} necesita también un script que implemente {nameof(IDeathHandler)}");
        }
    }

    public void TakeDamage(int amount)
    {
        // Si ya está muerto, ignoramos cualquier daño adicional
        if (_isDead) return;

        _currentHealth -= amount;

        if (bloodParticles != null)
        {
            bloodParticles.Play();
        }

        if (sfxSource != null && damageClips.Length > 0 && _currentHealth > 0)
        {
            int randomIndex = Random.Range(0, damageClips.Length);
            sfxSource.PlayOneShot(damageClips[randomIndex]);
        }

        if (_currentHealth <= 0)
        {
            _isDead = true; // Activamos el candado
            _currentHealth = 0;

            if (!gameObject.CompareTag("Player") && bloodParticles != null)
            {
                bloodParticles.transform.SetParent(null);
                Destroy(bloodParticles.gameObject, 2f);
            }

            ReproducirSonidoMuerte();
            _deathHandler.Die();
        }
    }

    private void ReproducirSonidoMuerte()
    {
        if (deathClip != null && sfxSource != null)
        {
            GameObject audioObj = new GameObject("DeathAudioTemp");
            audioObj.transform.position = transform.position;

            AudioSource tempSource = audioObj.AddComponent<AudioSource>();
            tempSource.clip = deathClip;

            tempSource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            tempSource.spatialBlend = sfxSource.spatialBlend;

            tempSource.Play();
            Destroy(audioObj, deathClip.length);
        }
    }

    public void ResetState()
    {
        _currentHealth = maxHealth;
        _isDead = false; // Reiniciamos el candado al respawnear (Jugador)

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