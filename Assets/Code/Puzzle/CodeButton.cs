
using UnityEngine;

public class CodeButton : Interactable
{
    [Header("Puzzle Reference")]
    [SerializeField] private CodePuzzle codePuzzle;

    [Header("Keypad Reference")]
    [SerializeField] private CodeKeypad codeKeypad;

    public override void Interact()
    {
        if (codeKeypad == null)
        {
            Debug.LogWarning("CodeKeypad is not assigned.");
            return;
        }

        codeKeypad.Open(this);
    }

    public bool TryCode(string code)
    {
        if (codePuzzle == null)
        {
            Debug.LogWarning("CodePuzzle is not assigned.");
            return false;
        }

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
