using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progreso del Juego")]
    public int currentFloor = 1;

    [Header("Estado del jugador")]
    public int savedHealth = 6;

    void Awake()
    {
        // Singleton para asegurar que solo exista un GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AdvanceToNextFloor()
    {
        currentFloor++;
    }
}