using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Configuración de la Mazmorra")]
    public int totalRooms = 10;
    public float roomDistance = 12f; // Distancia en unidades entre centros de habitaciones

    [Header("Prefabs de Habitaciones")]
    public GameObject startRoomPrefab; // Habitación Inicial
    public GameObject normalRoomPrefab; // Habitación Normal
    public GameObject bossRoomPrefab;   // Habitación del Jefe

    private HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        Vector2Int startPos = Vector2Int.zero;

        // 1. Crear habitación inicial
        roomPositions.Add(startPos);
        roomQueue.Enqueue(startPos);

        Vector2Int[] directions = new Vector2Int[]
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
        };

        // Lista temporal para almacenar todas las posiciones generadas
        List<Vector2Int> createdRooms = new List<Vector2Int>();
        createdRooms.Add(startPos);

        // 2. Bucle de expansión
        while (roomPositions.Count < totalRooms && roomQueue.Count > 0)
        {
            Vector2Int current = roomQueue.Dequeue();

            foreach (Vector2Int dir in directions)
            {
                if (Random.value > 0.5f && roomPositions.Count < totalRooms)
                {
                    Vector2Int nextPos = current + dir;

                    if (!roomPositions.Contains(nextPos))
                    {
                        roomPositions.Add(nextPos);
                        roomQueue.Enqueue(nextPos);
                        createdRooms.Add(nextPos);
                    }
                }
            }
        }

        // 3. Encontrar la habitación con la MAYOR DISTANCIA al Inicio (Start)
        Vector2Int bossPos = startPos;
        int maxDistance = 0;

        foreach (Vector2Int pos in createdRooms)
        {
            // Distancia Manhattan: |X1 - X2| + |Y1 - Y2|
            int distance = Mathf.Abs(pos.x - startPos.x) + Mathf.Abs(pos.y - startPos.y);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                bossPos = pos;
            }
        }

        // 4. Instanciar todas las habitaciones en la escena
        foreach (Vector2Int pos in createdRooms)
        {
            if (pos == startPos)
            {
                InstantiateRoom(pos, startRoomPrefab);
            }
            else if (pos == bossPos)
            {
                InstantiateRoom(pos, bossRoomPrefab);
            }
            else
            {
                InstantiateRoom(pos, normalRoomPrefab);
            }
        }
    }
    void InstantiateRoom(Vector2Int gridPos, GameObject prefab)
    {
        // Convertir la coordenada de la cuadrícula a posición en la Escena (X, Y)
        Vector3 worldPosition = new Vector3(gridPos.x * roomDistance, gridPos.y * roomDistance, 0f);
        Instantiate(prefab, worldPosition, Quaternion.identity, transform);
    }
}