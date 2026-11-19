
using UnityEngine;

public class CodePuzzle : MonoBehaviour
{
    [Header("Code Settings")]
    [SerializeField] private string firstCodePart = "38";
    [SerializeField] private string secondCodePart = "71";

    public bool CheckCode(string code)
    {
        return code == firstCodePart + secondCodePart;
    }
}
