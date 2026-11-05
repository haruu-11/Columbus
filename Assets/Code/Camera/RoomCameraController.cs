
using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    [Header("Camera Follow")]
    [SerializeField] private Transform player;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float maxYaw = 20f;
    [SerializeField] private float maxPitch = 10f;

    private GameObject[] _currentlyHiddenObjects;
    private Quaternion _baseRotation;

    public void ChangeRoom(Transform cameraPoint)
    {
        if (cameraPoint == null)
            return;

        ShowPreviousObjects();

        // Mantener la posición y establecer la rotación inicial
        transform.SetPositionAndRotation(
            cameraPoint.position,
            cameraPoint.rotation
        );

        _baseRotation = cameraPoint.rotation;

        CameraPoint cameraData = cameraPoint.GetComponent<CameraPoint>();

        if (cameraData != null)
        {
            _currentlyHiddenObjects = cameraData.ObjectsToHide;
            HideCurrentObjects();
        }
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;

        Vector3 localDirection =
            Quaternion.Inverse(_baseRotation) * direction;

        float yaw = Mathf.Atan2(
            localDirection.x,
            localDirection.z
        ) * Mathf.Rad2Deg;

        float horizontalDistance = new Vector2(
            localDirection.x,
            localDirection.z
        ).magnitude;

        float pitch = -Mathf.Atan2(
            localDirection.y,
            horizontalDistance
        ) * Mathf.Rad2Deg;

        yaw = Mathf.Clamp(yaw, -maxYaw, maxYaw);
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        Quaternion targetRotation = _baseRotation *
            Quaternion.Euler(pitch, yaw, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void HideCurrentObjects()
    {
        if (_currentlyHiddenObjects == null)
            return;

        foreach (GameObject obj in _currentlyHiddenObjects)
        {
            if (obj == null)
                continue;

            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
                meshRenderer.enabled = false;
        }
    }

    private void ShowPreviousObjects()
    {
        if (_currentlyHiddenObjects == null)
            return;

        foreach (GameObject obj in _currentlyHiddenObjects)
        {
            if (obj == null)
                continue;

            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
                meshRenderer.enabled = true;
        }

        _currentlyHiddenObjects = null;
    }
}
