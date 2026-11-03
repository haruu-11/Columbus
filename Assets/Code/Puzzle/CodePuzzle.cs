using UnityEngine;

public class CodePuzzle : MonoBehaviour
{
    [Header("Code Settings")]
    [SerializeField] private string firstCodePart = "38";
    [SerializeField] private string secondCodePart = "71";

    private bool _hasFirstPart;
    private bool _hasSecondPart;

    public bool HasCompleteCode => _hasFirstPart && _hasSecondPart;

    public bool CheckCode(string code)
    {
        if (!HasCompleteCode)
            return false;

        return code == firstCodePart + secondCodePart;
    }

    public void CollectCodePart(int part)
    {
        if (part == 1)
            _hasFirstPart = true;
        else if (part == 2)
            _hasSecondPart = true;
    }
}