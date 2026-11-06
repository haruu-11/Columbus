using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    private GameObject[] _currentlyHiddenObjects;

    public void ChangeRoom(Transform cameraPoint)
    {
        if (cameraPoint == null)
            return;

        // Volver a mostrar los objetos de la cámara anterior
        ShowPreviousObjects();

        // Mover la cámara
        transform.SetPositionAndRotation(cameraPoint.position, cameraPoint.rotation);

        // Buscar qué objetos debe ocultar esta cámara
        CameraPoint cameraData = cameraPoint.GetComponent<CameraPoint>();

        if (cameraData != null)
        {
            _currentlyHiddenObjects = cameraData.ObjectsToHide;

            HideCurrentObjects();
        }
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