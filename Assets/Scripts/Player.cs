using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public PlayerInput playerInput;
    public Animator animator;
    public CapsuleCollider2D playerCollider;

    [Header("Movement Variables")]
    public float speed;
    public float jumpForce;
    public float jumpCutMultiplier = .5f;
    public float normalGravity;
    public float fallGravity;
    public float jumpGravity;

    public int direction = 1;

    //Inputs
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool jumpReleased;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Crouch Settings")]
    public Transform headCheck;
    public float headCheckRadius = .2f;

    [Header("Slide Settings")]
    public float slideDuration = .6f;
    public float slideSpeed = 10f;
    public float slideStopDuration = .2f;

    public float slideHeight;
    public Vector2 slideOffset;
    public float normalHeight;
    public Vector2 normalOffset;

    private bool isSliding;
    private bool slideInputLocked;
    private float slideTimer;
    private float slideStopTimer;


    private void Start()
    {
        rb.gravityScale = normalGravity;
    }

    private void Update()
    {
        TryStandup();

        if (!isSliding)
            Flip();

        HandleAnimations();
        HandleSlide();
    }

    private void FixedUpdate()
    {
        ApplyVariableGravity();
        CheckGrounded();

        if (!isSliding)
        {
            HandleMovement();
        }

        HandleJump();
    }

    private void HandleMovement()
    {
        float targetSpeed = moveInput.x * speed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if(jumpPressed && isGrounded) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            jumpReleased = false;
        }
        if (jumpReleased) {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
            jumpReleased = false;
        }
    }

    private void HandleSlide() 
    {
        if (isSliding) { 
            slideTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(slideSpeed * direction, rb.linearVelocity.y);

            //If we are done sliding
            if (slideTimer <= 0) { 
                isSliding = false;
                slideStopTimer = slideStopDuration;
                TryStandup();
            }
        }

        if(slideStopTimer > 0)
        {
            slideStopTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        //Start the Slide
        if (isGrounded && moveInput.y < -.1f && !isSliding && !slideInputLocked)
        {
            isSliding = true;
            slideInputLocked = true;
            slideTimer = slideDuration;
            SetColliderSlide();
        }

        if (slideStopTimer < 0 && moveInput.y >= -.1f)
        {
            slideInputLocked = false;
        }
    }
    void HandleAnimations()
    {
        bool isCrouching = animator.GetBool("isCrouching");

        animator.SetBool("isJumping", rb.linearVelocity.y > .1f);
        animator.SetBool("isSliding", isSliding);

        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        bool isMoving = Mathf.Abs(moveInput.x) > .1f && isGrounded; 

        animator.SetBool("isIdle", !isMoving && !isSliding && !isCrouching);
        animator.SetBool("isRunning", isMoving && !isSliding && !isCrouching);
    }



    void SetColliderNormal()
    {
       playerCollider.size = new Vector2(playerCollider.size.x, normalHeight);
       playerCollider.offset = normalOffset;
    }

    void SetColliderSlide()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, slideHeight);
        playerCollider.offset = slideOffset;
    }

    void TryStandup()
    {
        if (isSliding)
        {
            animator.SetBool("isSliding", false);
            return;
        }

        bool canStandUp = !Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);

        if (canStandUp)
        {
            SetColliderNormal();
            animator.SetBool("isSliding", false);
        }
        else
        {
            SetColliderSlide();
            animator.SetBool("isSliding", true);
        }
    }

    void ApplyVariableGravity()
    {
        if(rb.linearVelocity.y < -0.1f) { 
            rb.gravityScale = fallGravity;
        }
        else if(rb.linearVelocity.y > 0.1f) { 
            rb.gravityScale = jumpGravity;
        }
        else { 
            rb.gravityScale = normalGravity;
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void Flip()
    {
        if (moveInput.x > 0.1f)
        {
            direction = 1;
        }
        else if(moveInput.x < -0.1f)
        {
            direction = -1;
        }

        transform.localScale = new Vector3(direction, 1, 1);
    }



    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump (InputValue value)
    {
        if (value.isPressed) {
            jumpPressed = true;
            jumpReleased = false;
        }
        else { 
            jumpReleased = true;
        }
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);
    }
}
