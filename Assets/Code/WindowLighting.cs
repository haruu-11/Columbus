
using System.Collections;
using UnityEngine;

public class WindowLightning : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Renderer glassRenderer;
    [SerializeField] private Light lightningLight;
    [SerializeField] private AudioSource windSource;
    [SerializeField] private AudioSource thunderSource;

    [Header("Colores del vidrio")]
    [SerializeField] private Color normalColor = new Color(0.01f, 0.01f, 0.12f, 1f);
    [SerializeField] private Color flashColor = Color.white;

    [Header("Relampagos")]
    [SerializeField] private float minInterval = 5f;
    [SerializeField] private float maxInterval = 15f;
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField] private float whiteHoldDuration = 0.8f;
    [SerializeField] private float maxIntensity = 20f;

    private Material glassMaterial;
    private int colorProperty;
    private Color initialLightColor;
    private float initialLightIntensity;

    private void Start()
    {
        if (glassRenderer == null)
        {
            Debug.LogError("Falta asignar Glass Renderer.");
            enabled = false;
            return;
        }

        glassMaterial = glassRenderer.material;

        if (glassMaterial.HasProperty("_BaseColor"))
            colorProperty = Shader.PropertyToID("_BaseColor");
        else if (glassMaterial.HasProperty("_Color"))
            colorProperty = Shader.PropertyToID("_Color");
        else
        {
            Debug.LogError("El material no tiene propiedad de color compatible.");
            enabled = false;
            return;
        }

        if (lightningLight != null)
        {
            initialLightColor = lightningLight.color;
            initialLightIntensity = lightningLight.intensity;
            lightningLight.intensity = 0f;
        }

        SetGlassColor(normalColor);

        if (windSource != null && windSource.clip != null)
        {
            windSource.loop = true;
            if (!windSource.isPlaying)
                windSource.Play();
        }

        StartCoroutine(LightningRoutine());
    }



    private IEnumerator LightningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(minInterval, maxInterval)
            );

            // Trueno al comenzar el relampago.
            if (thunderSource != null && thunderSource.clip != null)
                thunderSource.Play();

            // PRIMER DESTELLO: muy intenso.
            SetGlassColor(flashColor);
            yield return SetLightIntensity(maxIntensity, 0.08f);

            // El vidrio vuelve a oscuro, pero la luz sigue iluminando.
            SetGlassColor(normalColor);

            // SEGUNDO DESTELLO: mas corto y menos intenso.
            yield return SetLightIntensity(maxIntensity * 0.65f, 0.06f);

            // Pequeña pausa entre descargas.
            yield return SetLightIntensity(maxIntensity * 0.12f, 0.10f);

            // TERCER DESTELLO: ultimo rebote.
            yield return SetLightIntensity(maxIntensity * 0.35f, 0.08f);

            // DESVANECIMIENTO: la luz cae lentamente durante 3 segundos.
            float elapsed = 0f;
            float fadeDuration = 3f;
            float startingIntensity = maxIntensity * 0.35f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / fadeDuration);

                // Caida progresiva: empieza intensa y termina suavemente.
                float intensity = startingIntensity * Mathf.Pow(1f - t, 2.2f);

                if (lightningLight != null)
                    lightningLight.intensity = intensity;

                yield return null;
            }

            if (lightningLight != null)
                lightningLight.intensity = 0f;
        }
    }

    private IEnumerator SetLightIntensity(float target, float duration)
    {
        if (lightningLight == null)
            yield break;

        float start = lightningLight.intensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            lightningLight.intensity = Mathf.Lerp(start, target, t);

            yield return null;
        }

        lightningLight.intensity = target;
    }



    private void SetGlassColor(Color color)
    {
        glassMaterial.SetColor(colorProperty, color);
    }

    private void OnDisable()
    {
        if (glassMaterial != null)
            SetGlassColor(normalColor);

        if (lightningLight != null)
            lightningLight.intensity = 0f;

        if (windSource != null)
            windSource.Stop();
    }
}
