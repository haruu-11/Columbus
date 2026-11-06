using System;
using System.Collections;
using UnityEngine;

public class Shotgun : Weapon
{
    [Header("References")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Corrección de Rotación")]
    [Tooltip("Marcar si este modelo necesita rotar al ser equipado.")]
    [SerializeField] private bool invertModelRotation = false;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0f, 180f, 0f);

    [Header("Animation")]
    [SerializeField] private bool animateOnShoot;
    [SerializeField] private bool animateOnReload;
    [SerializeField] private string shootAnimationTrigger = "Shoot";
    [SerializeField] private string idleAnimationState = "Idle";

    private Animator _weaponAnimator;

    [Header("Settings")]
    [SerializeField] private int pellets = 6;
    [SerializeField] private float spread = 8f;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float fireRate = 0.8f;
    [SerializeField] private int damage = 20;

    [Header("Ammo")]
    [SerializeField] private int magazineCapacity = 8;
    [SerializeField] private int currentAmmo = 8;
    [SerializeField] private float reloadStartDelay = 0.5f;
    [SerializeField] private float reloadInterval = 0.5f;

    private Coroutine _reloadCoroutine;
    private Collider[] _playerColliders;
    private Rigidbody _rigidbody;

    private PlayerFire _playerFire;
    private PlayerAim _playerAim;

    private bool _isReloading;
    private bool _pickedUp;
    private float nextFireTime;

    public override int CurrentAmmo => currentAmmo;
    public override int Damage => damage;

    private void Start()
    {
        _playerFire = GetComponentInParent<PlayerFire>();
        if (_playerFire == null && weaponHolder != null)
        {
            _playerFire = weaponHolder.GetComponentInParent<PlayerFire>();
        }

        _playerAim = GetComponentInParent<PlayerAim>();
        if (_playerAim == null && weaponHolder != null)
        {
            _playerAim = weaponHolder.GetComponentInParent<PlayerAim>();
        }

        if (_playerFire != null)
        {
            _playerColliders = _playerFire.GetComponentsInParent<Collider>();
            IgnorePlayerCollision();
        }

        _rigidbody = GetComponentInChildren<Rigidbody>();
        _weaponAnimator = GetComponentInChildren<Animator>(true);
    }

    private void IgnorePlayerCollision()
    {
        if (_playerColliders == null) return;
        Collider[] weaponColliders = GetComponentsInChildren<Collider>();

        foreach (Collider weaponCollider in weaponColliders)
        {
            foreach (Collider playerCollider in _playerColliders)
            {
                Physics.IgnoreCollision(weaponCollider, playerCollider);
            }
        }
    }

    public override void Pickup()
    {
        if (_pickedUp)
            return;

        _pickedUp = true;

        if (weaponHolder != null)
        {
            transform.SetParent(weaponHolder);
            transform.localPosition = Vector3.zero;

            // Respeta la escala original y aplica la rotación solo si está marcada
            if (invertModelRotation)
                transform.localRotation = Quaternion.Euler(rotationOffset);
            else
                transform.localRotation = Quaternion.identity;
        }

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }

        if (_playerFire != null)
        {
            _playerFire.SetWeapon(this);
        }

        Debug.Log("Groovy");
    }

    public override void Shoot()
    {
        if (_isReloading)
        {
            CancelReload();
        }

        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            Debug.Log("No ammo in magazine");
            return;
        }

        if (animateOnShoot && _weaponAnimator != null)
        {
            _weaponAnimator.ResetTrigger(shootAnimationTrigger);
            _weaponAnimator.SetTrigger(shootAnimationTrigger);
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        Debug.Log("Shoot - Ammo left: " + currentAmmo);

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = _playerAim != null ? (_playerAim.AimPoint - muzzle.position) : muzzle.forward;
            direction.y = 0f;
            direction.Normalize();

            Quaternion rotation = Quaternion.LookRotation(direction);

            float randomX = UnityEngine.Random.Range(-spread, spread);
            float randomY = UnityEngine.Random.Range(-spread, spread);

            rotation *= Quaternion.Euler(randomX, randomY, 0f);

            GameObject projectile = Instantiate(
                projectilePrefab,
                muzzle.position,
                rotation
            );

            Projectile projectileScript = projectile.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.Initialize(Damage);
            }

            Rigidbody rb = projectile.GetComponentInChildren<Rigidbody>();

            if (rb == null)
            {
                Destroy(projectile);
                return;
            }

            rb.linearVelocity = rotation * Vector3.forward * projectileSpeed;
        }
    }

    public override void Reload(Action onReload)
    {
        if (_isReloading)
            return;

        if (currentAmmo >= magazineCapacity)
            return;

        AmmoInventory ammoInventory = GetComponentInParent<AmmoInventory>();

        if (ammoInventory == null || !ammoInventory.HasAmmo())
            return;

        if (_weaponAnimator != null && animateOnReload)
        {
            _weaponAnimator.SetBool("isReloading", true);
        }

        _reloadCoroutine = StartCoroutine(ReloadOneByOne(ammoInventory, onReload));
    }

    private IEnumerator ReloadOneByOne(AmmoInventory ammoInventory, Action onReload)
    {
        _isReloading = true;

        yield return new WaitForSeconds(reloadStartDelay);

        while (currentAmmo < magazineCapacity && ammoInventory.HasAmmo())
        {
            currentAmmo++;
            ammoInventory.TryUseAmmo();
            onReload?.Invoke();

            if (reloadInterval > 0f)
                yield return new WaitForSeconds(reloadInterval);
            else
                yield return null;
        }

        if (_weaponAnimator != null && animateOnReload)
        {
            _weaponAnimator.SetBool("isReloading", false);
        }

        _isReloading = false;
        _reloadCoroutine = null;
    }

    private void CancelReload()
    {
        if (_reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
            _reloadCoroutine = null;
        }

        _isReloading = false;

        if (_weaponAnimator != null && animateOnReload)
        {
            _weaponAnimator.SetBool("isReloading", false);
            _weaponAnimator.Play(idleAnimationState, 0, 0f);
        }
    }

    public override void Drop(Vector3 dropPosition)
    {
        if (_isReloading)
        {
            CancelReload();
        }

        _pickedUp = false;

        transform.SetParent(null);
        transform.position = dropPosition + transform.forward * 0.8f;
        transform.rotation = Quaternion.identity;

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
        }

        Debug.Log("Weapon dropped");
    }
}