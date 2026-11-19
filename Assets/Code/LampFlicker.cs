
using UnityEngine;
using System.Collections;

public class LampFlicker : MonoBehaviour
{
    [Header("Referencia")]
    [SerializeField] private Light lampLight;

    [Header("Intensidad")]
    [SerializeField] private float normalIntensity = 3f;
    [SerializeField] private float minIntensity = 0.2f;
    [SerializeField] private float maxIntensity = 3.5f;

    [Header("Intervalos")]
    [SerializeField] private float minWait = 1f;
    [SerializeField] private float maxWait = 5f;

    [Header("Parpadeos")]
    [SerializeField] private int minFlickers = 2;
    [SerializeField] private int maxFlickers = 5;
    [SerializeField] private float flickerSpeed = 0.06f;
    [SerializeField] private float blackoutChance = 0.25f;

    private void Start()
    {
        if (lampLight == null)
            lampLight = GetComponent<Light>();

        if (lampLight == null)
        {
            Debug.LogError("LampFlicker: no se encontró una Light.");
            enabled = false;
            return;
        }

        normalIntensity = lampLight.intensity;
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // La lámpara permanece estable durante un tiempo aleatorio.
            lampLight.intensity = normalIntensity;

            yield return new WaitForSeconds(
                Random.Range(minWait, maxWait)
            );

            // Una serie de parpadeos irregulares.
            int flickers = Random.Range(minFlickers, maxFlickers + 1);

            for (int i = 0; i < flickers; i++)
            {
                lampLight.intensity = Random.Range(
                    minIntensity,
                    maxIntensity
                );

                yield return new WaitForSeconds(
                    Random.Range(flickerSpeed, flickerSpeed * 2f)
                );

                lampLight.intensity = normalIntensity;

                yield return new WaitForSeconds(
                    Random.Range(0.04f, 0.15f)
                );
            }

            // Ocasionalmente se apaga por completo.
            if (Random.value < blackoutChance)
            {
                lampLight.intensity = 0f;

                yield return new WaitForSeconds(
                    Random.Range(0.2f, 0.7f)
                );

                lampLight.intensity = normalIntensity;
            }
        }
    }

    private void OnDisable()
    {
        if (lampLight != null)
            lampLight.intensity = normalIntensity;
    }
}
