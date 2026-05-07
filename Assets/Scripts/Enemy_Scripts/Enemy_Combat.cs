using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy_Combat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;

    private EnemyConfig config;
    private Enemy enemy;
    private float lastAttackTime;
    private StateMachine stateMachine;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        config = enemy.Config;
        stateMachine = enemy.StateMachine;
    }

    private void Update()
    {
        if(Keyboard.current.qKey.isPressed)
        {
            PreformDodge();
        }
    }
    public bool CanMeleeAttack() => Time.time >= lastAttackTime + config.meleeCooldown;

    public void PreformMeleeAttack()
    {
        lastAttackTime = Time.time;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, config.meleeRange, config.targetLayer);
        if (!hit)
            return;


        Health health = hit.GetComponentInChildren<Health>();
        if (health != null)
            health.ChangeHealth(-config.meleeDamage, transform.position);
    }

    public void PreformDodge()
    {
        stateMachine.ChangeState(new DodgeState(enemy));
    }
}
