
using UnityEngine;

public class NoteInteractable : Interactable
{
    [Header("UI Reference")]
    [SerializeField] private GameObject notePanel;

    [Header("Puzzle Reference")]
    [SerializeField] private CodePuzzle codePuzzle;

    private bool _wasCollected;

    public override void Interact()
    {
        notePanel.SetActive(true);

        if (!_wasCollected)
        {
            codePuzzle.CollectCodePart(2);
            _wasCollected = true;
        }
    }

    public void CloseNote()
    {
        notePanel.SetActive(false);
    }
}

