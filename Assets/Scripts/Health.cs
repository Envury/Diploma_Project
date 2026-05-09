using System;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public event Action<Vector2> OnDamaged;
    public event Action<Vector2> OnDeath;

    public int health;
    public int maxHealth;
    public Image healthBar;

    private void Start()
    {
        health = maxHealth;
    }


    public void ChangeHealth(int amount, Vector2 sourcePosition)
    {
        health += amount;

        if (health > maxHealth)
            health = maxHealth;

        else if (health <= 0)
        {
            OnDeath?.Invoke(sourcePosition);
            if (healthBar != null)
                healthBar.fillAmount = 0;
        }

        else if (amount < 0)
        {
            OnDamaged?.Invoke(sourcePosition);
            if (healthBar != null)
                healthBar.fillAmount = (float)health / maxHealth;
        }
    }
}
