
using UnityEngine;

public class HatchInteractable : Interactable
{
    [SerializeField] private NPCVoice npcVoice;

    public override void Interact()
    {
        npcVoice.PlayDialogue();
    }
}
