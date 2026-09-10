using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progreso del Juego")]
    public int currentFloor = 1;

    void Awake()
    {
        // Singleton para asegurar que solo exista un GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Evita que se destruya al cargar otra escena
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