using UnityEngine;

public class Door : Interactable
{
    [Header("Door Settings")]
    [SerializeField] private bool requiresKey = true;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;

    [Header("Required Item")]
    [SerializeField] private KeyData requiredKey;

    [Header("Audio SFX (Bloque C)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoAbrir;
    [SerializeField] private AudioClip sonidoCerrar;
    [SerializeField] private AudioClip sonidoTrabada; // NUEVO: Audio para puerta con llave

    private bool _isOpen;
    private Quaternion _closedRotation;
    private Quaternion _openRotation;

    private void Start()
    {
        _closedRotation = transform.rotation;
        _openRotation = _closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    private void Update()
    {
        Quaternion targetRotation = _isOpen ? _openRotation : _closedRotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            openSpeed * Time.deltaTime
        );
    }

    public override void Interact()
    {
        if (requiresKey)
        {
            Inventory inventory = FindFirstObjectByType<Inventory>();

            if (inventory == null)
            {
                Debug.LogWarning("No se encontró un Inventory en la escena.");
                return;
            }

            if (!inventory.HasItem(requiredKey))
            {
                Debug.Log("Necesitas una llave para abrir esta puerta.");

                // Reproduce el sonido de picaporte trabado con variación
                if (audioSource != null && sonidoTrabada != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.volume = Random.Range(0.85f, 1.0f);
                    audioSource.PlayOneShot(sonidoTrabada);
                }
                return; // Corta la ejecución para que no se abra
            }
        }

        _isOpen = !_isOpen;

        if (audioSource != null)
        {
            // Variación de tono y volumen para abrir/cerrar
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.volume = Random.Range(0.85f, 1.0f);

            if (_isOpen)
            {
                if (sonidoAbrir != null) audioSource.PlayOneShot(sonidoAbrir);
                Debug.Log("Puerta abierta");
            }
            else
            {
                if (sonidoCerrar != null) audioSource.PlayOneShot(sonidoCerrar);
                Debug.Log("Puerta cerrada");
            }
        }
    }
}