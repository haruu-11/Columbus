using UnityEngine;

public class CodeClue : Interactable
{
    [Header("Clue Settings")]
    [SerializeField] private CodePuzzle codePuzzle;
    [SerializeField] private int codePart;

    private bool _wasCollected;

    public override void Interact()
    {
        if (_wasCollected)
            return;

        codePuzzle.CollectCodePart(codePart);
        _wasCollected = true;

        Debug.Log($"Code part {codePart} collected!");
    }
}