using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    public Health health;

    [Header("Death FX")]
    [SerializeField] private GameObject[] deathParts;
    [SerializeField] private float spawnForce = 5;
    [SerializeField] private float torque = 5f;
    [SerializeField] private float lifetime = 2;

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

        enemy.StateMachine.ChangeState(new DamagedState(enemy, knockbackDir));
    }

    private void HandleDeath(Vector2 sourcePosition)
    {
        // For the dummy in spawn. Need to optimize this later.
        foreach (GameObject prefab in deathParts)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0.5f, 1)).normalized;
            GameObject part = Instantiate(prefab, transform.position, rotation);

            Rigidbody2D rb = part.GetComponent<Rigidbody2D>();

            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(.5f, 1)).normalized;
            rb.linearVelocity = randomDirection * spawnForce;
            rb.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);

            Destroy(part, lifetime);
        }
        
        //For all other enemies.
        enemy.StateMachine.ChangeState(new DeathState(enemy));
    }

    private void Die() { Destroy(gameObject); }
}
