using UnityEngine;

public class DeathState : State
{
    protected override string AnimBoolName => "isDead";

    public DeathState(Enemy enemy) : base(enemy) {}
}
