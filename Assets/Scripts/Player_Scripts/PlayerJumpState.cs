using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player) : base(player) { }



   public override void Enter()
    {
        base.Enter();
        animator.SetBool("isJumping", true);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, player.jumpForce);

        JumpPressed = false;
        JumpReleased = false;
    }

    public override void Update()
    {
        base.Update();

        if (player.isGrounded && rb.linearVelocity.y <= 0.1f)
            player.ChangeState(player.idleState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.ApplyVariableGravity();

        if (JumpReleased && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * player.jumpCutMultiplier);
            JumpReleased = false;
        }

        //float speed = player.speed;
        //float targetSpeed = speed * MoveInput.x;
        //rb.linearVelocity = new Vector2(player.direction * targetSpeed, rb.linearVelocity.y);
        //Horizontal movement handling in the air isnt needed
    }

    public override void Exit()
    {
        base.Exit();
        animator.SetBool("isJumping", false);
    }
}
