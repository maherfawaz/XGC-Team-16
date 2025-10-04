using System.Collections;
using UnityEngine;

public class MovementV2 : MonoBehaviour {

    [Header("Info")]
    [SerializeField][ReadOnly] private bool isGrounded;
    [SerializeField][ReadOnly] private bool isFalling;

    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;

    private Vector3 desiredDirection;
    private Vector3 targetPosition;

    private float offsetY;
    private Vector3 characterCenter;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField][ReadOnly] private Vector3 moveDirection;
    [SerializeField][ReadOnly] private float velocityY;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 5f;

    [Header("Dash")]
    [SerializeField][ReadOnly] private bool isDashing;
    [SerializeField][ReadOnly] private bool canDash;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float fallingThreshold = -0.1f;

    [Header("Teleport")]
    [SerializeField] private float respawnDuration = 0.5f;
    [SerializeField][ReadOnly] private bool isTeleporting = false;

    [Header("Developer Stuffs")]
    [SerializeField] private float raycastLength = 5f;

    private void Awake() {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Start() {
        offsetY = characterController.height * 0.5f;
    }

    void Update() {

        // CAN MOVE WHEN NOT TELEPORTING
        if (!isTeleporting) {
            ApplyMovement();
        }
    }

    private void ApplyMovement() {

        // CHECK IF GROUNDED
        isGrounded = characterController.isGrounded;
        animator.SetBool("IsGrounded", isGrounded);

        if (isGrounded && velocityY < 0f) {
            velocityY = -2f;
        }

        // CHECK IF FALLING
        isFalling = velocityY < fallingThreshold;
        animator.SetBool("IsFalling", isFalling);

        // SKIP MOVEMENT INPUT IF DASHING
        if (isDashing) {
            return;
        }

        // READ INPUT
        Vector3 input3D = playerInput.Movement.Move2D.ReadValue<Vector3>();
        Vector3 input2D = new Vector2(input3D.x, input3D.y);
        desiredDirection = new Vector3(input2D.x, input2D.y, 0f);

        // HORIZONTAL MOVEMENT ONLY IN X AXIS
        moveDirection = new Vector3(desiredDirection.x, 0f, 0f);

        // MOVEMENT ANIMATION SPEED
        if (isGrounded) {
            animator.SetFloat("Speed", Mathf.Abs(moveDirection.x));
            canDash = true;
        }

        // ROTATE/LOOK AT DIRECTION
        if (moveDirection.x != 0f) {
            this.transform.forward = moveDirection.x > 0 ? Vector3.right : Vector3.left;
        }

        // JUMP
        if (playerInput.Movement.Jump.WasPressedThisFrame() && isGrounded) {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // DASH
        if (playerInput.Movement.Dash.WasPressedThisFrame() && canDash) {
            Vector3 dashDir = desiredDirection.magnitude > 0.1f ? desiredDirection : Vector3.right * transform.forward.x;
            StartCoroutine(Dash(dashDir));
        }

        // APPLY GRAVITY
        velocityY += gravity * Time.deltaTime;

        // CALCULATE MOVEMENT
        targetPosition = (moveDirection * moveSpeed) + (Vector3.up * velocityY);

        targetPosition.z = 0f;
        characterController.Move(targetPosition * Time.deltaTime);

        // RESET Z AXIS POSITION
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        DrawMoveDirection();
    }

    private IEnumerator Dash(Vector3 direction) {

        isDashing = true;
        canDash = false;
        animator.SetTrigger("Dash");

        direction.z = 0f;
        direction.Normalize();

        float elapsedTime = 0f;
        Vector3 dashVelocity = direction * dashSpeed;

        float originalGravity = velocityY;
        velocityY = 0f;

        while (elapsedTime < dashDuration) {

            // MOVE PLAYER TO DASH POSITION
            characterController.Move(dashVelocity * Time.deltaTime);

            // LOCK Z AXIS POSITION
            Vector3 pos = transform.position;
            pos.z = 0f;
            transform.position = pos;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // RESTORE VELOCITY WHEN FALLING
        if (originalGravity < 0f && !isGrounded) {
            velocityY = originalGravity * 0.5f;
        }

        isDashing = false;
    }

    // USED BY CHECKPOINT SYSTEM
    public float GetVelocityY() {
        
        return velocityY;
    }

    public IEnumerator ResetPosition(Vector3 position) {

        isTeleporting = true;

        // PAUSE BEFORE TELEPORTING
        yield return new WaitForSeconds(respawnDuration);

        this.transform.position = position;
        velocityY = 0f;

        Debug.Log("Teleporting...");

        // PAUSE AFTER TELEPORTING
        yield return new WaitForSeconds(respawnDuration);

        Debug.Log("Teleporting Done!");

        isTeleporting = false;
    }

    private void DrawMoveDirection() {

        // DEBUG MOVE DIRECTION
        characterCenter = this.transform.position;
        characterCenter.y += offsetY;

        Debug.DrawRay(characterCenter, moveDirection * raycastLength, Color.blue);
        Debug.DrawRay(characterCenter, desiredDirection * raycastLength, Color.green);
    }

    private void OnEnable() {
        playerInput.Enable();
    }

    private void OnDisable() {
        playerInput.Disable();
    }
}