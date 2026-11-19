
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject menuPausa;

    private bool _estaPausado;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_estaPausado)
                Reanudar();
            else
                Pausar();
        }
    }

    public void Pausar()
    {
        _estaPausado = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        menuPausa.SetActive(true);
    }

    public void Reanudar()
    {
        _estaPausado = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        menuPausa.SetActive(false);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("Menu");
    }
}
