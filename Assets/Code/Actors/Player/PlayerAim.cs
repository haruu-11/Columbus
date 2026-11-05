using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask groundMask;

    private bool _isAiming;
    private Vector3 _aimPoint;

    public bool IsAiming => _isAiming;
    public Vector3 AimPoint => _aimPoint;

    private void Awake()
    {
        // Si no se asignó la cámara en el Inspector, buscamos la cámara principal o activa automáticamente
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void Update()
    {
        _isAiming = Mouse.current != null && Mouse.current.rightButton.isPressed;

        // Verificación continua por si cambia la cámara activa en el sistema de habitaciones
        if (cam == null || !cam.gameObject.activeInHierarchy)
        {
            // Busca entre las cámaras activas de la escena actual
            Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (Camera c in allCameras)
            {
                if (c.gameObject.activeInHierarchy)
                {
                    cam = c;
                    break;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!_isAiming || cam == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Usamos la posición exacta del jugador en el eje Y para mantener el plano de impacto
        Plane aimPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        if (aimPlane.Raycast(ray, out float distance))
        {
            _aimPoint = ray.GetPoint(distance);

            Vector3 direction = _aimPoint - transform.position;
            direction.y = 0f; // Evita totalmente que el personaje se incline hacia arriba o abajo

            if (direction.sqrMagnitude > 0.001f)
            {
                // Rotación suave para que no pegue saltos al apuntar
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
            }
        }
    }
}