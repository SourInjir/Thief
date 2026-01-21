using UnityEngine;
using UnityEngine.InputSystem;

public class Thief : MonoBehaviour
{
    private const float DefaultMoveSpeed = 4f;
    private const float DefaultTurnSpeed = 360f;
    private const float DefaultMouseSensitivity = 2f;
    private const float DefaultJumpHeight = 1.2f;
    private const float DefaultGravity = -9.81f;
    private const float DefaultGroundCheckDistance = 0.2f;
    private const float DefaultGroundCheckOffset = 0.1f;
    private const float MinInputSqrMagnitude = 0.01f;
    private const float MinVerticalVelocity = 0.001f;
    private const float Zero = 0f;
    private const float Two = 2f;

    private PlayerInput _playerInput;

    [SerializeField] private float _moveSpeed = DefaultMoveSpeed;
    [SerializeField] private float _turnSpeed = DefaultTurnSpeed;
    [SerializeField] private float _mouseSensitivity = DefaultMouseSensitivity;
    [SerializeField] private float _jumpHeight = DefaultJumpHeight;
    [SerializeField] private float _gravity = DefaultGravity;
    [SerializeField] private float _groundCheckDistance = DefaultGroundCheckDistance;
    [SerializeField] private float _groundCheckOffset = DefaultGroundCheckOffset;

    private Transform _cameraTransform;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _verticalVelocity;
    private bool _isGrounded;
    private bool _jumpRequested;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _cameraTransform = Camera.main != null ? Camera.main.transform : null;

        _playerInput.Player.Move.performed += OnMove;
        _playerInput.Player.Move.canceled += OnMove;
        _playerInput.Player.Look.performed += OnLook;
        _playerInput.Player.Look.canceled += OnLook;
        _playerInput.Player.Jump.performed += OnJump;
    }

     private void OnEnable()
    {
        _playerInput.Enable();
    }


    private void Update()
    {
        HandleLookRotation();
        HandleMovement();
        HandleMovementRotation();
        UpdateGroundedState();
        HandleJump();
        ApplyGravity();
        ApplyVerticalMovement();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = GetMoveDirection();
        if (moveDirection.sqrMagnitude < MinInputSqrMagnitude)
        {
            return;
        }

        transform.position += moveDirection * _moveSpeed * Time.deltaTime;
    }

    private void HandleLookRotation()
    {
        if (_lookInput.sqrMagnitude < MinInputSqrMagnitude)
        {
            return;
        }

        float yaw = _lookInput.x * _mouseSensitivity;
        transform.Rotate(Vector3.up, yaw);
    }

    private void HandleMovementRotation()
    {
        if (_lookInput.sqrMagnitude >= MinInputSqrMagnitude)
        {
            return;
        }

        Vector3 moveDirection = GetMoveDirection();
        if (moveDirection.sqrMagnitude < MinInputSqrMagnitude)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
    }

    private void UpdateGroundedState()
    {
        _isGrounded = CheckGrounded();
        if (_isGrounded && _verticalVelocity < Zero)
        {
            _verticalVelocity = Zero;
        }
    }

    private bool CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * _groundCheckOffset;
        float distance = _groundCheckDistance + _groundCheckOffset;
        return Physics.Raycast(origin, Vector3.down, distance);
    }

    private void HandleJump()
    {
        if (!_jumpRequested)
        {
            return;
        }

        _jumpRequested = false;

        if (!_isGrounded)
        {
            return;
        }

        _verticalVelocity = Mathf.Sqrt(_jumpHeight * -Two * _gravity);
    }

    private void ApplyGravity()
    {
        if (_isGrounded && _verticalVelocity <= Zero)
        {
            return;
        }

        _verticalVelocity += _gravity * Time.deltaTime;
    }

    private void ApplyVerticalMovement()
    {
        if (Mathf.Abs(_verticalVelocity) < MinVerticalVelocity)
        {
            return;
        }

        transform.position += Vector3.up * _verticalVelocity * Time.deltaTime;
    }

    private Vector3 GetMoveDirection()
    {
        if (_moveInput.sqrMagnitude < MinInputSqrMagnitude)
        {
            return Vector3.zero;
        }

        Vector3 forward = GetCameraForward();
        Vector3 right = GetCameraRight();

        Vector3 direction = forward * _moveInput.y + right * _moveInput.x;
        return direction.normalized;
    }

    private Vector3 GetCameraForward()
    {
        if (_cameraTransform == null)
        {
            return transform.forward;
        }

        Vector3 forward = _cameraTransform.forward;
        forward.y = Zero;
        return forward.normalized;
    }

    private Vector3 GetCameraRight()
    {
        if (_cameraTransform == null)
        {
            return transform.right;
        }

        Vector3 right = _cameraTransform.right;
        right.y = Zero;
        return right.normalized;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jumpRequested = true;
        }
    }
}
