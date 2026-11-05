using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerAim), typeof(PlayerFire))]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerAim playerAim;
    private PlayerFire playerFire;
    private CharacterController characterController;

    [Header("Input Settings (Opcional)")]
    [SerializeField] private InputActionReference moveActionReference; 

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAim = GetComponent<PlayerAim>();
        playerFire = GetComponent<PlayerFire>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (playerFire != null)
        {
            playerFire.OnFire += TriggerShoot;
            playerFire.OnReload += TriggerReload;
            playerFire.OnWeaponChanged += UpdateWeaponState;
        }
    }

    private void OnDestroy()
    {
        if (playerFire != null)
        {
            playerFire.OnFire -= TriggerShoot;
            playerFire.OnReload -= TriggerReload;
            playerFire.OnWeaponChanged -= UpdateWeaponState;
        }
    }

    private void Update()
    {
        UpdateMovementParameters();
    }

    private void UpdateMovementParameters()
    {
        if (animator == null) return;

        bool isMoving = playerMovement != null && playerMovement.IsMoving;
        bool isAiming = playerAim != null && playerAim.IsAiming;
        bool hasWeapon = playerFire != null && playerFire.GetCurrentWeapon() != null;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isAiming", isAiming);
        animator.SetBool("hasWeapon", hasWeapon);
    }

    private void TriggerShoot()
    {
        if (animator != null) animator.SetTrigger("Shoot");
    }

    private void TriggerReload()
    {
        if (animator != null) animator.SetTrigger("Reload");
    }

    private void UpdateWeaponState() { }
}