using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private PlayerFire playerFire;
    private void Start()
    {
        UpdateText();
    }

    private void OnEnable()
    {
        playerFire.OnFire += UpdateText;
        playerFire.OnReload += UpdateText;
        playerFire.OnWeaponChanged += UpdateText;
        ammoInventory.OnAmmoObtained += UpdateText;
    }

    private void OnDisable()
    {
        playerFire.OnFire -= UpdateText;
        playerFire.OnReload -= UpdateText;
        playerFire.OnWeaponChanged -= UpdateText;
        ammoInventory.OnAmmoObtained -= UpdateText;
    }

    private void UpdateText()
    {
        Weapon currentWeapon = playerFire.GetCurrentWeapon();
        if (currentWeapon == null)
        {
            ammoText.enabled = false;
            return;
        }

        ammoText.enabled = true;
        ammoText.text = currentWeapon.CurrentAmmo + " / " + ammoInventory.Ammo;
    }
}
