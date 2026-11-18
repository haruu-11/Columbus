using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerAim))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float aimingSpeed = 2.5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField, Range(0f, 0.3f)] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float speedChangeRate = 10f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Input References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private float walkingStepInterval = 0.55f;
    [SerializeField] private float aimingStepInterval = 0.65f;
    [SerializeField] private float sprintStepInterval = 0.32f;

    private CharacterController _controller;
    private PlayerAim _playerAim;
    private Transform _cameraTransform;

    private Vector2 _moveDirection;
    private float _verticalVelocity;
    private bool _sprint;

    private float _speed;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _footstepTimer;

    public bool IsMoving => _moveDirection.sqrMagnitude > 0.01f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerAim = GetComponent<PlayerAim>();

        UpdateCameraReference();
    }

    private void Update()
    {
        ReadInput();
        ApplyGravity();
        Move();
        UpdateFootsteps();
    }

    private void ReadInput()
    {
        if (moveAction != null && moveAction.action != null)
        {
            _moveDirection = moveAction.action.ReadValue<Vector2>();
        }

        if (sprintAction != null && sprintAction.action != null)
        {
            _sprint = sprintAction.action.IsPressed();
        }
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void UpdateCameraReference()
    {
        if (_cameraTransform == null ||
            !_cameraTransform.gameObject.activeInHierarchy)
        {
            if (Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }
    }

    private void Move()
    {
        UpdateCameraReference();

        bool isAiming = _playerAim != null && _playerAim.IsAiming;

        float targetSpeed = isAiming
            ? aimingSpeed
            : (_sprint ? sprintSpeed : moveSpeed);

        if (_moveDirection == Vector2.zero)
        {
            targetSpeed = 0f;
        }

        Vector3 currentVelocity = _controller.velocity;
        currentVelocity.y = 0f;

        float currentHorizontalSpeed = currentVelocity.magnitude;
        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(
                currentHorizontalSpeed,
                targetSpeed,
                Time.deltaTime * speedChangeRate
            );

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        if (_moveDirection == Vector2.zero)
        {
            _speed = 0f;
        }

        Vector3 targetDirection = Vector3.zero;

        if (_moveDirection != Vector2.zero &&
            _cameraTransform != null)
        {
            Vector3 cameraForward = _cameraTransform.forward;
            Vector3 cameraRight = _cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            targetDirection =
                cameraForward * _moveDirection.y +
                cameraRight * _moveDirection.x;

            if (targetDirection != Vector3.zero)
            {
                _targetRotation =
                    Mathf.Atan2(targetDirection.x, targetDirection.z) *
                    Mathf.Rad2Deg;

                float rotation = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    _targetRotation,
                    ref _rotationVelocity,
                    rotationSmoothTime
                );

                transform.rotation =
                    Quaternion.Euler(0f, rotation, 0f);
            }
        }

        Vector3 motion =
            targetDirection.normalized * _speed +
            Vector3.up * _verticalVelocity;

        _controller.Move(motion * Time.deltaTime);
    }

    private void UpdateFootsteps()
    {
        if (footstepAudio == null || footstepAudio.clip == null)
        {
            return;
        }

        Vector3 horizontalVelocity = _controller.velocity;
        horizontalVelocity.y = 0f;

        bool isActuallyMoving = horizontalVelocity.magnitude > 0.5f;

        if (!_controller.isGrounded || !isActuallyMoving)
        {
            _footstepTimer = 0f;
            return;
        }

        bool isAiming = _playerAim != null && _playerAim.IsAiming;

        float interval = isAiming
            ? aimingStepInterval
            : (_sprint ? sprintStepInterval : walkingStepInterval);

        _footstepTimer += Time.deltaTime;

        if (_footstepTimer >= interval)
        {
            footstepAudio.PlayOneShot(footstepAudio.clip);
            _footstepTimer = 0f;
        }
    }
}