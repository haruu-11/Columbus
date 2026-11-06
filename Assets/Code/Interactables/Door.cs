using UnityEngine;

public class Door : Interactable
{
    [Header("Door Settings")]
    [SerializeField] private bool requiresKey = true;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;

    [Header("Required Item")]
    [SerializeField] private KeyData requiredKey;

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
                return;
            }
        }

        _isOpen = !_isOpen;

        if (_isOpen)
        {
            Debug.Log("Puerta abierta");
        }
        else
        {
            Debug.Log("Puerta cerrada");
        }
    }
}
