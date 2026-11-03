using UnityEngine;

public class CodeButton : Interactable
{
    [Header("Puzzle Reference")]
    [SerializeField] private CodePuzzle codePuzzle;

    public override void Interact()
    {
        if (!codePuzzle.HasCompleteCode)
        {
            Debug.Log("You don't have the complete code.");
            return;
        }

        Debug.Log("Enter the code.");
    }

    public bool TryCode(string code)
    {
        if (!codePuzzle.CheckCode(code))
        {
            Debug.Log("Incorrect code.");
            return false;
        }

        Debug.Log("Correct code! Mechanism activated.");
        ActivateMechanism();
        return true;
    }

    private void ActivateMechanism()
    {
     
    }
}