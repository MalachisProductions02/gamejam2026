using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float maxHealth = 3f;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemigo recibió " + damage + " de daño.");
        Debug.Log("Vida del enemigo: " + currentHealth);

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("El enemigo murió.");

        Destroy(gameObject);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}