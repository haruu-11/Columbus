
using UnityEngine;

public class HatchInteractable : Interactable
{
    [SerializeField] private NPCVoice npcVoice;

    public override void Interact()
    {
        Debug.Log("HatchInteractable: Interact ejecutado");

        if (npcVoice == null)
        {
            Debug.LogError("NPCVoice no está asignado en HatchInteractable.");
            return;
        }

        npcVoice.PlayDialogue();
    }
}