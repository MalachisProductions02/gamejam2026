using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Configuración Base de la Mazmorra")]
    public int baseRooms = 8;             // Habitaciones en el Piso 1
    public int roomsPerFloor = 2;         // Cuántas habitaciones extra se suman por cada piso
    public float roomDistance = 12f;

    [Header("Ajustes de Generación")]
    [Range(0.1f, 1.0f)]
    public float expansionChance = 0.6f;

    [Header("Prefabs de Habitaciones")]
    public GameObject startRoomPrefab;
    public GameObject[] roomPrefabs;
    public GameObject bossRoomPrefab;

    private int totalRooms;
    private HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    void Start()
    {
        /* Calcular habitaciones según el piso actual del GameManager
        int currentFloor = 1;
        if (GameManager.Instance != null)
        {
            currentFloor = GameManager.Instance.currentFloor;
        }

        totalRooms = baseRooms + (currentFloor - 1) * roomsPerFloor;
        Debug.Log($"Generando Piso {currentFloor} con {totalRooms} habitaciones.");

        GenerateDungeon();
        
        (suma habitaciones por piso)
        */
        totalRooms = baseRooms;

        Debug.Log($"Generando nivel con {totalRooms} habitaciones.");

        GenerateDungeon();
    }

    void GenerateDungeon()
    {
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

        List<Vector2Int> createdRooms = new List<Vector2Int> { startPos };

        while (roomPositions.Count < totalRooms)
        {
            if (roomQueue.Count == 0)
            {
                Vector2Int randomCreated = createdRooms[Random.Range(0, createdRooms.Count)];
                roomQueue.Enqueue(randomCreated);
            }

            Vector2Int current = roomQueue.Dequeue();
            ShuffleDirections(directions);

            foreach (Vector2Int dir in directions)
            {
                if (roomPositions.Count >= totalRooms)
                    break;

                Vector2Int nextPos = current + dir;

                if (!roomPositions.Contains(nextPos))
                {
                    if (Random.value <= expansionChance || roomQueue.Count == 0)
                    {
                        roomPositions.Add(nextPos);
                        roomQueue.Enqueue(nextPos);
                        createdRooms.Add(nextPos);
                    }
                }
            }
        }

        // Buscar habitación del Boss (la más lejana)
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