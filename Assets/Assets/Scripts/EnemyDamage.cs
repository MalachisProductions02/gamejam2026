using UnityEngine;
public class EnemyDamage : MonoBehaviour
{
    [Header("Daño")]
    [SerializeField] private int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }
}