using UnityEngine;

public class PlayerCrouchState : PlayerState
{
    public PlayerCrouchState(Player player) : base(player) { }



    public override void Enter()
    {
        base.Enter();
        animator.SetBool("isCrouching", true);
        player.SetColliderSlide();
    }

    public override void Update()
    {
        base.Update();

        if (JumpPressed)
        {
            player.ChangeState(player.jumpState);
        }
        else if (!player.isCrouching && !player.CheckForCeiling())
        {
            player.ChangeState(player.idleState);
        }
    }



    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(Mathf.Abs(MoveInput.x) > .1f)
        {
            rb.linearVelocity = new Vector2(player.direction * player.speed / 2, rb.linearVelocity.y);
        }
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }



    public override void Exit()
    {
        base.Exit();
        animator.SetBool("isCrouching", false);
        player.SetColliderNormal();
    }
}

