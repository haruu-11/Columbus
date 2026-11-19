
using UnityEngine;

public class NoteInteractable : Interactable
{
    [Header("UI Reference")]
    [SerializeField] private GameObject notePanel;

    public override void Interact()
    {
        notePanel.SetActive(true);
    }

    public void CloseNote()
    {
        notePanel.SetActive(false);
    }
}
