using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progreso del Juego")]
    public int currentFloor = 1;

    [Header("Configuración Alpha")]
    public int maxFloors = 2;

    [Header("Estado del jugador")]
    public int savedHealth = 6;

    void Awake()
    {
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
        if (currentFloor < maxFloors)
        {
            currentFloor++;
            Debug.Log("Avanzando al piso: " + currentFloor);
        }
        else
        {
            Debug.Log("Ya se alcanzó el último piso: " + currentFloor);
        }
    }

    public bool HasNextFloor()
    {
        return currentFloor < maxFloors;
    }
}