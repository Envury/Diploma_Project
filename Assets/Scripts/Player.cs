using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public PlayerInput playerInput;
    public Animator animator;

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


    private void Start()
    {
        rb.gravityScale = normalGravity;
    }

    private void Update()
    {
        Flip();
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        ApplyVariableGravity();
        CheckGrounded();
        HandleMovement();
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

    void HandleAnimations()
    {
        animator.SetBool("isJumping", rb.linearVelocity.y > .1f);

        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        animator.SetBool("isIdle", Mathf.Abs(moveInput.x) < .1f && isGrounded);
        animator.SetBool("isRunning", Mathf.Abs(moveInput.x) > .1f && isGrounded);
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
    }
}
