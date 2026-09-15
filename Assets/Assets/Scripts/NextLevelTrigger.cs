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
        if (GameManager.Instance != null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                Player player = playerObject.GetComponent<Player>();

                if (player != null)
                {
                    GameManager.Instance.savedHealth = player.GetCurrentHealth();
                }
            }

            GameManager.Instance.AdvanceToNextFloor();
        }

        SceneManager.LoadScene(dungeonSceneName);
    }
}