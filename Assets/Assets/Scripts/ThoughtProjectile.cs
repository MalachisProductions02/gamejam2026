using UnityEngine;

public class ThoughtProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 3f;

    private float damage;
    private Vector2 direction;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError(
                "ThoughtProjectile necesita un Rigidbody2D."
            );
        }
    }

    public void Initialize(Vector2 direction, float damage)
    {
        this.direction = direction.normalized;
        this.damage = damage;

        if (rb != null)
        {
            rb.velocity = this.direction * speed;
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Enemigo
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }


        // Pared

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}