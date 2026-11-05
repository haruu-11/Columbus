using System;
using System.Collections;
using UnityEngine;

public class Shotgun : Weapon
{
    [Header("References")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Audio SFX (Bloque C)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;   // Arrastrar Disparo Arma_2.wav
    [SerializeField] private AudioClip emptyClip;   // Arrastrar Disparar sin Ammo_2.wav
    [SerializeField] private AudioClip reloadClip;  // Arrastrar Recarga_2.wav

    [Header("Animation")]
    [SerializeField] private bool animateOnShoot;
    [SerializeField] private bool animateOnReload;
    [SerializeField] private string shootAnimationTrigger = "Shoot";
    [SerializeField] private string reloadAnimationTrigger = "Reload";
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
        _playerFire = weaponHolder.GetComponentInParent<PlayerFire>();
        _playerAim = weaponHolder.GetComponentInParent<PlayerAim>();

        _playerColliders = _playerFire.GetComponentsInParent<Collider>();

        IgnorePlayerCollision();

        _rigidbody = GetComponentInChildren<Rigidbody>();
        _weaponAnimator = GetComponentInChildren<Animator>(true);
    }

    private void IgnorePlayerCollision()
    {
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

        transform.SetParent(weaponHolder);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

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
            if (audioSource != null && emptyClip != null)
            {
                audioSource.PlayOneShot(emptyClip);
            }
            // Agregamos un pequeño delay para que no se spammee el sonido de vacío si el jugador aprieta muy rápido
            nextFireTime = Time.time + 0.2f;
            return;
        }

        if (animateOnShoot && _weaponAnimator != null)
        {
            _weaponAnimator.ResetTrigger(shootAnimationTrigger);
            _weaponAnimator.SetTrigger(shootAnimationTrigger);
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        // REPRODUCCIÓN DEL DISPARO
        if (audioSource != null && shootClip != null)
        {
            audioSource.PlayOneShot(shootClip);
        }

        Debug.Log("Shoot - Ammo left: " + currentAmmo);

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = _playerAim.AimPoint - muzzle.position;
            direction.y = 0f;
            direction.Normalize();

            Quaternion rotation = Quaternion.LookRotation(direction);
            float randomX = UnityEngine.Random.Range(-spread, spread);
            float randomY = UnityEngine.Random.Range(-spread, spread);
            rotation *= Quaternion.Euler(randomX, randomY, 0f);

            GameObject projectile = Instantiate(projectilePrefab, muzzle.position, rotation);
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
            _weaponAnimator.ResetTrigger(reloadAnimationTrigger);
            _weaponAnimator.Play(idleAnimationState, 0, 0f);
            _weaponAnimator.Update(0f);
        }

        Debug.Log("Reload interrupted");
    }

    public override void Reload(Action onReload)
    {
        if (_isReloading) return;
        if (currentAmmo >= magazineCapacity) return;

        AmmoInventory ammoInventory = GetComponentInParent<AmmoInventory>();

        if (ammoInventory == null)
        {
            Debug.LogError("AmmoInventory not found in Player");
            return;
        }

        if (!ammoInventory.HasAmmo())
        {
            Debug.Log("No more ammo on reserve");
            return;
        }

        if (animateOnReload && _weaponAnimator != null)
        {
            _weaponAnimator.ResetTrigger(reloadAnimationTrigger);
            _weaponAnimator.SetTrigger(reloadAnimationTrigger);
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

            // REPRODUCCIÓN DE LA RECARGA (Suena por cada cartucho insertado)
            if (audioSource != null && reloadClip != null)
            {
                audioSource.PlayOneShot(reloadClip);
            }

            onReload?.Invoke();
            Debug.Log("Reloading... " + currentAmmo + "/" + magazineCapacity);

            if (reloadInterval > 0f)
                yield return new WaitForSeconds(reloadInterval);
            else
                yield return null;
        }

        _isReloading = false;
        _reloadCoroutine = null;

        Debug.Log("Reload complete");
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