using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Configuración de la Mazmorra")]
    [Tooltip("Cantidad exacta de habitaciones a generar (incluyendo Inicio y Boss).")]
    public int totalRooms = 10;
    public float roomDistance = 12f;

    [Header("Ajustes de Generación")]
    [Range(0.1f, 1.0f)]
    [Tooltip("Probabilidad de extenderse a un vecino. Valores altos crean mapas más compactos; valores bajos crean caminos más lineales.")]
    public float expansionChance = 0.6f;

    [Header("Prefabs de Habitaciones")]
    public GameObject startRoomPrefab;
    public GameObject[] roomPrefabs;
    public GameObject bossRoomPrefab;

    private HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // Limpiar estado
        roomPositions.Clear();
        roomQueue.Clear();

        Vector2Int startPos = Vector2Int.zero;

        roomPositions.Add(startPos);
        roomQueue.Enqueue(startPos);

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        List<Vector2Int> createdRooms = new List<Vector2Int>();
        createdRooms.Add(startPos);

        // Bucle de expansión garantizado
        while (roomPositions.Count < totalRooms)
        {
            // Si la cola se vacía por mala suerte en el Random, tomamos una posición existente al azar para seguir expandiendo
            if (roomQueue.Count == 0)
            {
                Vector2Int randomCreated = createdRooms[Random.Range(0, createdRooms.Count)];
                roomQueue.Enqueue(randomCreated);
            }

            Vector2Int current = roomQueue.Dequeue();

            // Mezclar direcciones para mayor variedad
            ShuffleDirections(directions);

            foreach (Vector2Int dir in directions)
            {
                if (roomPositions.Count >= totalRooms)
                    break;

                Vector2Int nextPos = current + dir;

                if (!roomPositions.Contains(nextPos))
                {
                    // La primera conexión desde un nodo atascado se fuerza para evitar bucles infinitos
                    if (Random.value <= expansionChance || roomQueue.Count == 0)
                    {
                        roomPositions.Add(nextPos);
                        roomQueue.Enqueue(nextPos);
                        createdRooms.Add(nextPos);
                    }
                }
            }
        }

        // Buscar la habitación más alejada (Boss)
        Vector2Int bossPos = startPos;
        int maxDistance = 0;

        foreach (Vector2Int pos in createdRooms)
        {
            int distance = Mathf.Abs(pos.x - startPos.x) + Mathf.Abs(pos.y - startPos.y);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                bossPos = pos;
            }
        }

        // Instanciar prefabs
        List<GameObject> availableRooms = new List<GameObject>(roomPrefabs);

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
                if (availableRooms.Count > 0)
                {
                    int randomIndex = Random.Range(0, availableRooms.Count);
                    GameObject selectedRoom = availableRooms[randomIndex];
                    InstantiateRoom(pos, selectedRoom);
                    availableRooms.RemoveAt(randomIndex);
                }
                else
                {
                    // Si tienes menos prefabs únicos que salas solicitadas, reutiliza un prefab aleatorio
                    GameObject fallbackPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
                    InstantiateRoom(pos, fallbackPrefab);
                }
            }
        }
    }

    void InstantiateRoom(Vector2Int gridPos, GameObject prefab)
    {
        Vector3 worldPosition = new Vector3(
            gridPos.x * roomDistance,
            gridPos.y * roomDistance,
            0f
        );

        Instantiate(prefab, worldPosition, Quaternion.identity, transform);
    }

    // Método auxiliar para desordenar las direcciones en cada paso
    void ShuffleDirections(Vector2Int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Vector2Int temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}