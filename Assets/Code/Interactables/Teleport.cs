using UnityEngine;

public class Teletransporte : MonoBehaviour
{
    [SerializeField] Transform destino;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (destino == null) { Debug.LogError("Falta asignar el destino"); return; }

        Transform jugador = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform.root;

        var cc = jugador.GetComponent<CharacterController>();
        var rb = jugador.GetComponent<Rigidbody>();

        if (cc != null) cc.enabled = false;

        jugador.position = destino.position;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;   
            rb.angularVelocity = Vector3.zero;
            rb.position = destino.position;
            Physics.SyncTransforms();
        }

        if (cc != null) cc.enabled = true;
    }
}