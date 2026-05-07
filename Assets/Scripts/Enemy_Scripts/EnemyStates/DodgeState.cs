using System;
using UnityEngine;

public class DodgeState : State
{
    private float dodgeVelocity;
    private float dodgeDuration;

    public DodgeState(Enemy enemy) : base(enemy)
    {
        dodgeVelocity = config.dodgeForce * -enemy.FacingDirection;
    }

    public override void Enter()
    {
        base.Enter();    
        dodgeDuration = config.dodgeDuration;
        rb.linearVelocity = new Vector2(dodgeVelocity, rb.linearVelocity.y);
    }

    public override void FixedUpdate()
    {
        dodgeDuration -= Time.fixedDeltaTime;
        if (dodgeDuration <= 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (!senses.IsAtCliff())
                stateMachine.ChangeState(new IdleState(enemy));
        }
    }
}
