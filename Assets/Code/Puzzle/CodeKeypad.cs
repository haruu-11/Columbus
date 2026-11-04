
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CodeKeypad : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text display;
    [SerializeField] private GameObject keypadPanel;

    [Header("Settings")]
    [SerializeField] private int maxDigits = 4;

    private string _currentCode = "";
    private CodeButton _targetButton;

    private void Update()
    {
        if (!keypadPanel.activeSelf)
            return;

        ReadKeyboardInput();
    }

    public void Open(CodeButton targetButton)
    {
        _targetButton = targetButton;
        _currentCode = "";
        UpdateDisplay();

        keypadPanel.SetActive(true);
    }

    public void Close()
    {
        keypadPanel.SetActive(false);
        _currentCode = "";
        _targetButton = null;
    }

    public void AddDigit(int digit)
    {
        if (_currentCode.Length >= maxDigits)
            return;

        _currentCode += digit.ToString();
        UpdateDisplay();
    }

    public void ClearCode()
    {
        _currentCode = "";
        UpdateDisplay();
    }

    public void DeleteLastDigit()
    {
        if (_currentCode.Length == 0)
            return;

        _currentCode = _currentCode.Substring(0, _currentCode.Length - 1);
        UpdateDisplay();
    }

    public void SubmitCode()
    {
        if (_currentCode.Length != maxDigits)
            return;

        if (_targetButton != null)
        {
            bool correct = _targetButton.TryCode(_currentCode);

            if (correct)
                Close();
            else
                ClearCode();
        }
    }

    private void UpdateDisplay()
    {
        display.text = _currentCode.PadRight(maxDigits, '_');
    }

    private void ReadKeyboardInput()
    {
        if (Keyboard.current == null)
            return;

        Keyboard keyboard = Keyboard.current;

        for (int i = 0; i <= 9; i++)
        {
            if (GetDigitKey(keyboard, i).wasPressedThisFrame)
                AddDigit(i);

            if (GetNumpadKey(keyboard, i).wasPressedThisFrame)
                AddDigit(i);
        }

        if (keyboard.backspaceKey.wasPressedThisFrame)
            DeleteLastDigit();

        if (keyboard.enterKey.wasPressedThisFrame ||
            keyboard.numpadEnterKey.wasPressedThisFrame)
            SubmitCode();

        if (keyboard.escapeKey.wasPressedThisFrame)
            Close();
    }

    private KeyControl GetDigitKey(Keyboard keyboard, int digit)
    {
        return digit switch
        {
            0 => keyboard.digit0Key,
            1 => keyboard.digit1Key,
            2 => keyboard.digit2Key,
            3 => keyboard.digit3Key,
            4 => keyboard.digit4Key,
            5 => keyboard.digit5Key,
            6 => keyboard.digit6Key,
            7 => keyboard.digit7Key,
            8 => keyboard.digit8Key,
            9 => keyboard.digit9Key,
            _ => keyboard.digit0Key
        };
    }

    private KeyControl GetNumpadKey(Keyboard keyboard, int digit)
    {
        return digit switch
        {
            0 => keyboard.numpad0Key,
            1 => keyboard.numpad1Key,
            2 => keyboard.numpad2Key,
            3 => keyboard.numpad3Key,
            4 => keyboard.numpad4Key,
            5 => keyboard.numpad5Key,
            6 => keyboard.numpad6Key,
            7 => keyboard.numpad7Key,
            8 => keyboard.numpad8Key,
            9 => keyboard.numpad9Key,
            _ => keyboard.numpad0Key
        };
    }
}
