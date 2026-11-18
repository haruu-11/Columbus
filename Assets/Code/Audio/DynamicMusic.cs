
using UnityEngine;

public class DynamicMusic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private AudioSource[] musicSources;

    [Header("Volume Settings")]
    [SerializeField, Range(0f, 1f)] private float normalVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float lowHealthVolume = 0.55f;
    [SerializeField, Range(0f, 1f)] private float criticalHealthVolume = 0.25f;
    [SerializeField] private float fadeSpeed = 0.5f;

    private float[] _originalVolumes;

    private void Awake()
    {
        _originalVolumes = new float[musicSources.Length];

        for (int i = 0; i < musicSources.Length; i++)
        {
            if (musicSources[i] != null)
                _originalVolumes[i] = musicSources[i].volume;
        }
    }

    private void Update()
    {
        if (playerHealth == null)
            return;

        float health = playerHealth.HealthPercentage;
        float targetVolume = normalVolume;

        if (health <= 0.25f)
            targetVolume = criticalHealthVolume;
        else if (health <= 0.5f)
            targetVolume = lowHealthVolume;

        for (int i = 0; i < musicSources.Length; i++)
        {
            if (musicSources[i] == null)
                continue;

            musicSources[i].volume = Mathf.MoveTowards(
                musicSources[i].volume,
                _originalVolumes[i] * targetVolume,
                fadeSpeed * Time.deltaTime
            );
        }
    }
}
