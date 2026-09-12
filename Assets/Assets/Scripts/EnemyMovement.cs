using System;
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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Configuración para juego 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.speed = speed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = stoppingDistance;

        // Buscar automáticamente al jugador
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el Tag 'Player'.");
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        Vector2Int enemyRoom = GetRoomPosition(transform.position);
        Vector2Int playerRoom = GetRoomPosition(player.position);

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
        int x = Mathf.RoundToInt(worldPosition.x / roomDistance);
        int y = Mathf.RoundToInt(worldPosition.y / roomDistance);

        return new Vector2Int(x, y);
    }
}