using UnityEngine;

public class IdleState : State
{

    private Transform target;
    protected override string AnimBoolName => "isIdling";

    public IdleState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        base.Enter();
        rb.linearVelocity = Vector2.zero;
    }

    public override void FixedUpdate()
    {
        //1. Check for target
        target = senses.GetChaseTarget();

        if (!target)
        {
            stateMachine.ChangeState(new PatrolState(enemy));
            return;
        }

        enemy.FaceTarget(target);

        //2. Check if we are within attack range
        if (senses.IsInMeleeRange(target) && combat.CanMeleeAttack())
        {
            stateMachine.ChangeState(new MeleeAttackState(enemy));
            return;
        }

        //3. Check if we have reached our target
        float distance = Mathf.Abs(target.position.x - enemy.transform.position.x);
        if (distance <= config.turnThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        //4. Check for obstacles
        if (senses.IsHittingWall() || senses.IsAtCliff())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        //5. We HAVE a target, we are NOT at the target, and we are NOT hitting an obstacle. Time to chase!
        stateMachine.ChangeState(new ChaseState(enemy));
    }
}
