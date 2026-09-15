using UnityEngine;

public class LifeUI : MonoBehaviour
{
    [Header("Jugador")]
    public Player player;

    [Header("Corazones")]
    public GameObject[] hearts;

    void Update()
    {
        if (player == null)
            return;

        int currentHealth = player.GetCurrentHealth();

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < currentHealth);
        }
    }
}