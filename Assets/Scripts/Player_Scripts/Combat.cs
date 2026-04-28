using UnityEngine;

public class Combat : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage;
    public float attackRadius = .5f;
    public float attackcooldown = 1.5f;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public Animator hitFX;

    public Player player;

    public bool CanAttack => Time.time >= attackTimer;
    private float attackTimer;



    public void AttackAnimationFinished()
    {
        player.AttackAnimationFinished();
    }

    public void Attack()
    {
        if (!CanAttack)
            return;

        attackTimer = Time.time + attackcooldown;

        Collider2D enemy = Physics2D.OverlapCircle(attackPoint.position, attackRadius, enemyLayer);

        if (enemy != null) {
            hitFX.Play("HitFX");
            enemy.gameObject.GetComponent<Health>().ChangeHealth(-damage, transform.position);
        }
    }
}
