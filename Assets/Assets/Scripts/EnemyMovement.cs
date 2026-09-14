using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Dungeon")]
    [SerializeField] private float roomDistance = 12f;

    [Header("Movimiento")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float stoppingDistance = 0.2f;

    private NavMeshAgent agent;
    private Transform player;

    private bool navMeshReady = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Buscar automáticamente al jugador
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el Tag 'Player'.");
        }

        // Configuración para juego 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.speed = speed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = stoppingDistance;

        // El NavMesh todavía puede no existir.
        // Esperamos a que esté listo.
        agent.enabled = false;

        StartCoroutine(WaitForNavMesh());
    }

    private IEnumerator WaitForNavMesh()
    {
        NavMeshHit hit;

        while (!NavMesh.SamplePosition(
            transform.position,
            out hit,
            2f,
            NavMesh.AllAreas))
        {
            yield return null;
        }

        // Colocar exactamente al enemigo sobre el NavMesh
        transform.position = hit.position;

        // Activar el agente ahora que el NavMesh existe
        agent.enabled = true;

        // Asegurarnos de que está colocado sobre el NavMesh
        agent.Warp(hit.position);

        navMeshReady = true;
    }

    private void Update()
    {
        if (!navMeshReady)
            return;

        if (player == null)
            return;

        // Seguridad adicional
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        Vector2Int enemyRoom =
            GetRoomPosition(transform.position);

        Vector2Int playerRoom =
            GetRoomPosition(player.position);

        if (enemyRoom == playerRoom)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();
        }
    }

    private Vector2Int GetRoomPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(
            worldPosition.x / roomDistance
        );

        int y = Mathf.RoundToInt(
            worldPosition.y / roomDistance
        );

        return new Vector2Int(x, y);
    }
}