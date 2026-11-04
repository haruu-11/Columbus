using UnityEngine;

public class pause : MonoBehaviour
{
    [SerializeField] GameObject menuPausa;

    public void Pausar()
    {
        Time.timeScale = 0.01f;
        menuPausa.SetActive(true);
    }
    public void Reaundar()
    {
        Time.timeScale = 1f;
        menuPausa.SetActive(false);
    }

}