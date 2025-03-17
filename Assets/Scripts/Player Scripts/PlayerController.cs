using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{  
    [Header("States")]
    [SerializeField] private PlayerState playerState;
    public IdleState idleState;
    public JumpState jumpState;
    public WalkState walkState;

    [Header("Movement")] 
    public float moveSpeed;
    public float groundDrag;
    
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool _readyToJump;
    
    [Header("Keybindings")]
    public KeyCode jumpKey = KeyCode.Space;
    
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool _isGrounded;
    
    
    public Transform orientation;
    private float _horizontalInput;
    private float _verticalInput;
    
    private Vector3 _movementDirection;
    
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        GroundCheck();
        MyInput();
        SpeedControl();
    }

    private void GroundCheck()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, (playerHeight * 0.5f) + 0.2f, whatIsGround);

        if (_isGrounded)
        {
            _rigidbody.linearDamping = groundDrag;
            _readyToJump = true;
        }
        else
            _rigidbody.linearDamping = 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(jumpKey) && _readyToJump && _isGrounded)
        {
            _readyToJump = false;
            
            Jump();
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        _movementDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (_isGrounded) 
            _rigidbody.AddForce(_movementDirection.normalized * (moveSpeed * 10f), ForceMode.Force);

        else if (!_isGrounded)
            _rigidbody.AddForce(_movementDirection.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            _rigidbody.linearVelocity = new Vector3(limitedVelocity.x, _rigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        
        _rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }
}
