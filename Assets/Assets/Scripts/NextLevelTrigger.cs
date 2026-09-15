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

            if (GameManager.Instance != null &&
                GameManager.Instance.HasNextFloor())
            {
                Debug.Log("Presiona 'E' para avanzar al siguiente piso.");
            }
            else
            {
                Debug.Log("Este es el último piso.");
            }
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
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("No existe GameManager.");
            return;
        }

        //AQUÍ PONES QUE SE ACABA EL JUEGO
        if (!GameManager.Instance.HasNextFloor())
        {
            Debug.Log("No hay más pisos. Este es el último.");
            return;
        }

        // Guardar la vida actual
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            Player player =
                playerObject.GetComponent<Player>();

            if (player != null)
            {
                GameManager.Instance.savedHealth =
                    player.GetCurrentHealth();
            }
        }

        // Aumentar contador de piso
        GameManager.Instance.AdvanceToNextFloor();

        // Cargar nuevamente la escena
        SceneManager.LoadScene(dungeonSceneName);
    }
}