/* Copyright © 2025 RUBEN RONQUILLO */

using UnityEngine;

public class Movement : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10.0f;
    private float turnAmount = 90.0f;

    private Vector3 moveDirection;
    private Vector3 lookDirection;

    private bool isSliding;

    [Space(5)]
    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private int jumpMaxCapacity = 1;
    [SerializeField] private int jumpsAvailable;

    [Space(5)]
    [Header("Gravity")]
    [SerializeField] private float gravityAmount = 5.0f;
    private float velocityY = 0.0f;

    [Space(5)]
    [Header("Collision")]
    [SerializeField] private float raycastDistance = 5f;

    private Vector3 groundSurface;
    private float groundAngle;
    private Vector3 groundProjection;

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        jumpsAvailable = jumpMaxCapacity;
    }

    void Update()
    {
        Move();
        Jump();
        GroundSurfaceAngle();
    }

    void Move()
    {
        if (isSliding)
        {
            /* MOVE ALONG SLIDING SURFACE */
            Vector3 direction = groundProjection;
            direction.y = velocityY;

            characterController.Move(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            /* CALCULATE MOVE DIRECTION */
            moveDirection = new Vector3(playerInput.Movement.Move2D.ReadValue<Vector3>().x, velocityY, playerInput.Movement.Move2D.ReadValue<Vector3>().z);
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        /* TURN PLAYER */
        float facing = moveDirection.x * turnAmount;
        if (facing > 0 || facing < 0)
        {
            this.transform.rotation = Quaternion.AngleAxis(moveDirection.x * turnAmount, transform.up);
        }
    }

    void Jump()
    {
        if (characterController.isGrounded)
        {
            /* RESET JUMPS AVAILABLE */
            jumpsAvailable = jumpMaxCapacity;
        }
        else
        {
            /* APPLY GRAVITY */
            velocityY -= gravityAmount * Time.deltaTime;
        }

        /* PERFORM JUMP */
        if (playerInput.Movement.Jump.WasPerformedThisFrame() && jumpsAvailable > 0) 
        {
            velocityY = jumpHeight;
            jumpsAvailable--;

            /* JUMPS AWAY FROM SLIDING SURFACE (TESTING) */
            if (isSliding)
            {
                Vector3 direction = groundSurface;
                direction.y = velocityY;

                characterController.Move(direction * moveSpeed * Time.deltaTime);
            }
        }
    }

    /* CALCULATE GROUND ANGLE */
    void GroundSurfaceAngle()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, -transform.up, out hit, raycastDistance))
        {
            groundSurface = hit.normal;
            groundAngle = Vector3.Angle(groundSurface, transform.up);
            groundProjection = Vector3.ProjectOnPlane(new Vector3(0f, velocityY, 0f), groundSurface);

            Debug.DrawRay(hit.point, groundProjection * raycastDistance, Color.blue);
        }

        isSliding = groundAngle > characterController.slopeLimit;

        Debug.DrawRay(this.transform.position, -transform.up * raycastDistance, Color.red);
    }

    void OnEnable()
    {
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }
}
