using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerAim))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Move speed while aiming")]
    [SerializeField] private float aimingSpeed = 2.5f;

    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField] private float sprintSpeed = 10f;

    [Tooltip("How fast the character turns to face movement direction")]
    [SerializeField][Range(0.0f, 0.3f)] private float rotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float speedChangeRate = 10.0f;

    [Header("Gravity")]
    [Tooltip("Fuerza de la gravedad aplicada al jugador")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Input References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;

    private CharacterController _controller;
    private PlayerAim _playerAim;
    private Transform _cameraTransform;

    private Vector2 _moveDirection;
    private float _verticalVelocity;

    private bool _sprint;
    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;

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
        if (_controller.isGrounded && _verticalVelocity < 0)
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
        if (_cameraTransform == null || !_cameraTransform.gameObject.activeInHierarchy)
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
        float targetSpeed = isAiming ? aimingSpeed : (_sprint ? sprintSpeed : moveSpeed);

        if (_moveDirection == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        float currentHorizontalSpeed = new Vector3(
            _controller.velocity.x,
            0.0f,
            _controller.velocity.z
        ).magnitude;

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

        // Si soltó el control, frenamos la velocidad horizontal inmediatamente
        if (_moveDirection == Vector2.zero)
        {
            _speed = 0.0f;
        }

        Vector3 targetDirection = Vector3.zero;

        if (_moveDirection != Vector2.zero && _cameraTransform != null)
        {
            Vector3 cameraForward = _cameraTransform.forward;
            Vector3 cameraRight = _cameraTransform.right;

            cameraForward.y = 0.0f;
            cameraRight.y = 0.0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            targetDirection = cameraForward * _moveDirection.y + cameraRight * _moveDirection.x;

            if (targetDirection != Vector3.zero)
            {
                _targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

                float rotation = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    _targetRotation,
                    ref _rotationVelocity,
                    rotationSmoothTime
                );

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        Vector3 motion = targetDirection.normalized * _speed + new Vector3(0.0f, _verticalVelocity, 0.0f);
        _controller.Move(motion * Time.deltaTime);
    }
}