using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    public PlayerState currentState;
    public PlayerIdleState idleState;
    public PlayerJumpState jumpState;
    public PlayerMoveState moveState;
    public PlayerCrouchState crouchState;
    public PlayerDamagedState damagedState;
    public PlayerDeathState deathState;
    public PlayerSlideState slideState;
    public PlayerAttackState attackState;

    [Header("Core Components")]
    public Combat combat;
    public Damage damage;
    public Health health;



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
    public Vector2 moveInput;
    public bool jumpPressed;
    public bool jumpReleased;
    public bool attackPressed;
    
    

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public bool isGrounded;



    [Header("Crouch Settings")]
    public Transform headCheck;
    public float headCheckRadius = .2f;
    public bool isCrouching;


    [Header("Slide Settings")]
    public float slideDuration = .6f;
    public float slideSpeed = 10f;
    public float slideStopDuration = .2f;

    public float slideHeight;
    public Vector2 slideOffset;
    public float normalHeight;
    public Vector2 normalOffset;

    private bool isSliding;



    private void Awake()
    {
        idleState = new PlayerIdleState(this);
        jumpState = new PlayerJumpState(this);
        moveState = new PlayerMoveState(this);
        crouchState = new PlayerCrouchState(this);
        slideState = new PlayerSlideState(this);
        attackState = new PlayerAttackState(this);
        damagedState = new PlayerDamagedState(this);
        deathState = new PlayerDeathState(this);
    }



    private void Start()
    {
        rb.gravityScale = normalGravity;

        ChangeState(idleState);
    }

    private void Update()
    {
        currentState.Update();

        if (!isSliding)
            Flip();

        HandleAnimations();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate();

        CheckGrounded();
    }






    public void ChangeState(PlayerState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }




    void HandleAnimations()
    {
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }




    public void SetColliderNormal()
    {
       playerCollider.size = new Vector2(playerCollider.size.x, normalHeight);
       playerCollider.offset = normalOffset;
    }

    public void SetColliderSlide()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, slideHeight);
        playerCollider.offset = slideOffset;
    }





    public void ApplyVariableGravity()
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

    public bool CheckForCeiling()
    {
        return Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
    }




    void Flip()
    {
        if(currentState == deathState)
            return;

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

    public void AttackAnimationFinished()
    {
        currentState.AttackAnimationFinished();
    }


    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnCrouch (InputValue value)
    {
        if (value.isPressed) 
            isCrouching = true;
        else 
            isCrouching = false;  
    }

    public void OnJump (InputValue value)
    {
        if (value.isPressed) {
            if(isGrounded && !CheckForCeiling())
                jumpPressed = true;

            jumpReleased = false;
        }
        else { 
            jumpReleased = true;
        }
    }

    public void OnAttack (InputValue value)
    {
        attackPressed = value.isPressed;
    }




    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);
    }
}
