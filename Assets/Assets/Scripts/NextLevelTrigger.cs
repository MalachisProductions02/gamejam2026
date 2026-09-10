using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTrigger : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre de la escena de juego que genera la mazmorra.")]
    public string dungeonSceneName = "DungeonScene";

    [Header("Tecla de Interacción")]
    public KeyCode interactKey = KeyCode.E;

    private bool isPlayerInside = false;

    void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(interactKey))
        {
            ChangeFloor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Debug.Log("Presiona 'E' para avanzar al siguiente piso.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    void ChangeFloor()
    {
        // Incrementar el número de piso en el GameManager si existe
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AdvanceToNextFloor();
        }

        // Recargar la escena de la mazmorra (o ir a la siguiente)
        SceneManager.LoadScene(dungeonSceneName);
    }
}