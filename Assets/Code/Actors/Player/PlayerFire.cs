
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAim))]
public class PlayerFire : MonoBehaviour
{
    public event Action OnFire;
    public event Action OnReload;
    public event Action OnWeaponChanged;

    private Weapon _currentWeapon;
    private PlayerAim _playerAim;

    private void Awake()
    {
        _playerAim = GetComponent<PlayerAim>();
    }

    private void Update()
    {
        // DISPARO
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && IsPlayerAiming() && _currentWeapon != null)
        {
            _currentWeapon.Shoot();
            OnFire?.Invoke();
        }

        // RECARGA
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame && _currentWeapon != null)
        {
            _currentWeapon.Reload(() => OnReload?.Invoke());
        }
    }

    public void SetWeapon(Weapon weapon)
    {
        _currentWeapon = weapon;
        OnWeaponChanged?.Invoke();
    }

    public Weapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    public void DropCurrentWeapon(Vector3 dropPosition)
    {
        if (_currentWeapon == null)
            return;

        _currentWeapon.Drop(dropPosition);

        _currentWeapon = null;
        OnWeaponChanged?.Invoke();
    }

    private bool IsPlayerAiming()
    {
        return _playerAim != null && _playerAim.IsAiming;
    }
}
