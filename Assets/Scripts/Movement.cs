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
    [SerializeField] private bool isDashing;

    Vector3 desiredDirection;
    [SerializeField] private float desiredDirectionLength = 5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20.0f;
    [SerializeField] private float dashDistance = 10.0f;
    [SerializeField] private int dashMaxCapacity = 1;
    [SerializeField] private int dashesAvailable;

    [SerializeField] private float dashDuration;

    [Space(5)]
    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2.0f;
    [Range(1, 3)]
    [SerializeField] private int jumpMaxCapacity = 1;
    [SerializeField] private int jumpsAvailable;

    [Space(5)]
    [Header("Wall Jump")]
    [SerializeField] private float offsetY = 1f;
    [SerializeField] private float wallRaycastDistance = 5f;
    //[SerializeField] private float wallJumpAngleLimit = 90f;
    [SerializeField] private float wallJumpAngle;

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
        dashesAvailable = dashMaxCapacity;
    }

    void Update()
    {
        Move();
        Dash();
        Jump();
        DesiredDirection();
        GroundSurfaceAngle();
        WallDetection();
    }

    /* [A/D KEYS] */
    void Move()
    {
        if (isSliding)
        {
            /* MOVE ALONG SLIDING SURFACE */
            Vector3 direction = groundProjection;
            direction.y = velocityY;

            characterController.Move(direction * moveSpeed * Time.deltaTime);
        }
        else if (isDashing)
        {
            Vector3 dashDirection = desiredDirection * dashDistance;
            //dashDirection += moveDirection;
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
        }
        else
        {
            /* CALCULATE MOVE DIRECTION */
            moveDirection = new Vector3(playerInput.Movement.Move2D.ReadValue<Vector3>().x, velocityY, playerInput.Movement.Move2D.ReadValue<Vector3>().z);
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        /* TURN PLAYER */
        float facing = moveDirection.x * turnAmount;
        if (facing >= 65f || facing <= -65f)
        {
            this.transform.rotation = Quaternion.AngleAxis(moveDirection.x * turnAmount, transform.up);
        }
    }

    /* [LEFT SHIFT] */
    void Dash()
    {
        Vector3 dashDirection = transform.forward * dashDistance;

        if (playerInput.Movement.Dash.WasPerformedThisFrame() && !isDashing)
        {
            dashDuration = 0f;
            isDashing = true;
        }

        if (isDashing)
        {
            if (dashDuration < (dashDistance * 0.8f))
            {
                dashDuration += dashSpeed * Time.deltaTime;
            }
            else
            {
                isDashing = false;
            }
        }
    }

    /* [SPACE BAR] */
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

    void WallDetection()
    {
        Vector3 position = this.transform.position;
        position.y += offsetY;

        RaycastHit hit;
        if (Physics.Raycast(position, transform.forward, out hit, wallRaycastDistance))
        {
            Vector3 surface = hit.normal;
            wallJumpAngle = Vector3.Angle(surface, transform.up);
            jumpsAvailable = jumpMaxCapacity;
        }
        else if (Physics.Raycast(position, -transform.forward, out hit, wallRaycastDistance))
        {
            Vector3 surface = hit.normal;
            wallJumpAngle = Vector3.Angle(surface, -transform.up);
            jumpsAvailable = jumpMaxCapacity;
        }
        else
        {
            wallJumpAngle = 0f;
        }

        Debug.DrawRay(position, transform.forward * wallRaycastDistance, Color.yellow);
        Debug.DrawRay(position, -transform.forward * wallRaycastDistance, Color.yellow);
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

    void DesiredDirection()
    {
        Vector3 position = this.transform.position;
        position.y += offsetY;

        desiredDirection = playerInput.Movement.Move2D.ReadValue<Vector3>();
        Debug.DrawRay(position, desiredDirection * desiredDirectionLength, Color.green);
    }

    public CharacterController GetCharacterController()
    {
        return characterController;
    }

    public void ResetPosition(Vector3 position)
    {
        this.transform.position = position;
    }

    public float GetVelocityY()
    {
        return velocityY;
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