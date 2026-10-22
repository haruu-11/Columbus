
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DynamicMusic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private AudioSource musicSource;

    [Header("Volume Settings")]
    [SerializeField, Range(0f, 1f)] private float normalVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float lowHealthVolume = 0.55f;
    [SerializeField, Range(0f, 1f)] private float criticalHealthVolume = 0.25f;
    [SerializeField] private float fadeSpeed = 0.5f;

    private void Awake()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (playerHealth == null || musicSource == null)
            return;

        float health = playerHealth.HealthPercentage;
        float targetVolume = normalVolume;

        if (health <= 0.25f)
            targetVolume = criticalHealthVolume;
        else if (health <= 0.5f)
            targetVolume = lowHealthVolume;

        musicSource.volume = Mathf.MoveTowards(
            musicSource.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );
    }
}
