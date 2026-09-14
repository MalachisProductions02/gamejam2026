using UnityEngine;

public class ThoughtProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 3f;

    private float damage;
    private Vector2 direction;

    public void Initialize(Vector2 direction, float damage)
    {
        this.direction = direction.normalized;
        this.damage = damage;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position +=
            (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy == null)
            return;

        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}