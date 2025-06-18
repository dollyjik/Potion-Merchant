using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{  
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
    
    [Header("Others")]
    public Transform cameraOrientation;
    private float _horizontalInput;
    private float _verticalInput;
    protected CharacterController CharController;
    protected Vector3 Velocity;
    
    
    public static PlayerController Instance { get; private set; } // Singleton instance

    // You can expose the Transform directly for easy access
    public Transform PlayerTransform => transform;

    private void Awake()
    {
        
        CharController = GetComponent<CharacterController>();
        if (Instance == null)
        {
            Instance = this;
            // Optional: If your player should persist across scenes, uncomment this
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }
    
    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            Jump();
        }
        GetInput();
        ApplyGravity();
    }

    private void FixedUpdate()
    {
        HandleMovement();
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

        moveDirection.y = 0;

        CharController.Move(moveDirection * (moveSpeed * Time.deltaTime));
    }

    public void Jump()
    {
        if (isGrounded)
        {
            Velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
