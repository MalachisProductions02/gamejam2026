using System.Collections;
using UnityEngine;

public class HealingChair : MonoBehaviour
{
    [Header("Curación")]
    [SerializeField] private float healInterval = 0.3f;

    private bool isPlayerInside = false;
    private bool isHealing = false;

    private Player player;

    private void Update()
    {
        if (!isPlayerInside)
            return;

        if (Input.GetKeyDown(KeyCode.E) && !isHealing)
        {
            StartCoroutine(HealPlayer());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.GetComponent<Player>();
        isPlayerInside = true;

        Debug.Log("Presiona E para curarte.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        isPlayerInside = false;
        player = null;
    }

    private IEnumerator HealPlayer()
    {
        if (player == null)
            yield break;

        isHealing = true;

        while (player.GetCurrentHealth() < player.GetMaxHealth())
        {
            player.Heal(1);

            yield return new WaitForSeconds(healInterval);
        }

        isHealing = false;

        Debug.Log("Vida completamente recuperada.");
    }
}