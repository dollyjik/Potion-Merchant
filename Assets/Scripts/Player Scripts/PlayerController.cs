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
    [SerializeField] private float moveSpeed;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    public bool isGrounded;
    
    [Header("Keybindings")]
    public KeyCode jumpKey = KeyCode.Space;
    
    [Header("")]
    private float _horizontalInput;
    private float _verticalInput;
    public Transform cameraOrientation;
    protected CharacterController CharController;
    protected Vector3 Velocity;

    private void Awake()
    {
        CharController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        //States
        idleState.Setup(CharController, this);
        jumpState.Setup(CharController, this);
        walkState.Setup(CharController, this);

        playerState = idleState;
    }

    private void Update()
    {
        GroundCheck();
        GetInput();
        playerState.Do();
        ApplyGravity();
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
        if (Input.GetKeyDown(jumpKey) && isGrounded)
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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void ApplyGravity()
    {
        if (isGrounded && Velocity.y < 0)
        {
            Velocity.y = -2f;
        }
        else
        {
            Velocity.y += gravity * Time.deltaTime;
        }
        CharController.Move(Velocity * Time.deltaTime);
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
    }

    public void HandleMovement()
    {
        Vector3 moveDirection = cameraOrientation.forward * _verticalInput 
                                + cameraOrientation.right * _horizontalInput;

        moveDirection.y = 0; // Prevent movement from affecting Y-axis
        //moveDirection.Normalize(); // Normalize to prevent diagonal speed boost

        CharController.Move(moveDirection * (moveSpeed * Time.deltaTime));
    }

    public void Jump()
    {
        if (isGrounded)
        {
            Velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
