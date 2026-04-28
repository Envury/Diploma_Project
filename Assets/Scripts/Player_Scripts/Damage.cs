using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private Player player;
    public Health health;

    [Header("Knockback Settings")]
    public float knockbackForce = 12;
    public float knockbackDuration = 0.2f;

    private void OnEnable()
    {
        health.OnDamaged += HandleDamage;
        health.OnDeath += HandleDeath;
    }


    private void OnDisable()
    {
        health.OnDamaged -= HandleDamage;
        health.OnDeath -= HandleDeath;
    }



    private void HandleDamage(Vector2 sourcePosition)
    {
        int knockbackDir = 0;
        knockbackDir = transform.position.x > sourcePosition.x ? 1 : -1;

        player.damagedState.SetParameters(knockbackDir);
        player.ChangeState(player.damagedState);
    }

    private void HandleDeath(Vector2 sourcePosition)
    {
        int knockbackDir = 0;
        knockbackDir = transform.position.x > sourcePosition.x ? 1 : -1;

        player.deathState.SetParameters(knockbackDir);
        player.ChangeState(player.deathState);
    }
}
