
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject panelControl;

    public void AbrirPanel()
    {
        panelControl.SetActive(true);
    }

    public void CerrarPanel()
    {
        panelControl.SetActive(false);
    }

    public GameObject panelCredits;
    public GameObject menuPrincipal;

    public void AbrirCredits()
    {
        panelCredits.SetActive(true);
        menuPrincipal.SetActive(false);
    }

    public void CerrarCredits()
    {
        panelCredits.SetActive(false);
        menuPrincipal.SetActive(true);
    }
}

