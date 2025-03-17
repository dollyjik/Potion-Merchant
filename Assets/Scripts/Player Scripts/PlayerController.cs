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
    public bool readyToJump;
    
    [Header("Keybindings")]
    public KeyCode jumpKey = KeyCode.Space;
    
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool isGrounded;
    
    
    public Transform orientation;
    private float _horizontalInput;
    private float _verticalInput;
    
    private Vector3 _movementDirection;
    
    private Rigidbody _rigidbody;

    private void Start()
    {
        //States
        idleState.Setup(_rigidbody, this);
        jumpState.Setup(_rigidbody, this);
        walkState.Setup(_rigidbody, this);

        playerState = idleState;
        
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        GroundCheck();
        GetInput();
        SpeedControl();
        playerState.Do();
        Debug.Log(playerState);
    }

    private void LateUpdate()
    {
        SelectState();
    }

    private void FixedUpdate()
    {
        playerState.FixedDo();
    }
    
    private void SelectState()
    {
        PlayerState oldState = playerState;

        if (isGrounded)
        {
            if (_horizontalInput == 0 && _verticalInput == 0)
            {
                playerState = idleState;
            }
            else
            {
                playerState = walkState;
            }
        }
        if (Input.GetKeyDown(jumpKey) && readyToJump && isGrounded)
        {
            playerState = jumpState;
        }
        

        if (oldState != playerState || oldState.IsComplete)
        {
            oldState.Exit();
            playerState.Initialise();
            playerState.Enter();
        }
    }
    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, (playerHeight * 0.5f) + 0.2f, whatIsGround);

        if (isGrounded)
        {
            _rigidbody.linearDamping = groundDrag;
            readyToJump = true;
        }
        else
            _rigidbody.linearDamping = 0f;
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        
    }

    public void MovePlayer()
    {
        _movementDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        if (isGrounded) 
            _rigidbody.AddForce(_movementDirection.normalized * (moveSpeed * 10f), ForceMode.Force);

        else if (!isGrounded)
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

    internal void Jump()
    {
        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        
        _rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

}
