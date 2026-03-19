using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player) { }


    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();

        if (JumpPressed)
            player.ChangeState(player.jumpState);
        
        else if (Mathf.Abs(MoveInput.x) < .1f)
            player.ChangeState(player.idleState);

        else if(player.isGrounded && player.isCrouching && Mathf.Abs(MoveInput.x) > .1f)
            player.ChangeState(player.slideState);

        else
            animator.SetBool("isRunning", true);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        float speed = player.speed;
        rb.linearVelocity = new Vector2(player.direction * speed, rb.linearVelocity.y);
    }

    public override void Exit() 
    { 
        base.Exit();

        animator.SetBool("isRunning", false);
    }
}
