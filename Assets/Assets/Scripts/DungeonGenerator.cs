using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Configuración Base de la Mazmorra")]
    public int baseRooms = 8;
    public int roomsPerFloor = 2;
    public float roomDistance = 12f;

    [Header("Ajustes de Generación")]
    [Range(0.1f, 1.0f)]
    public float expansionChance = 0.6f;

    [Header("Prefabs de Habitaciones")]
    public GameObject startRoomPrefab;
    public GameObject[] roomPrefabs;
    public GameObject bossRoomPrefab;

    public NavMeshSurface navMeshSurface;

    private int totalRooms;

    private HashSet<Vector2Int> roomPositions =
        new HashSet<Vector2Int>();

    private Dictionary<Vector2Int, GameObject> roomObjects =
        new Dictionary<Vector2Int, GameObject>();

    private Queue<Vector2Int> roomQueue =
        new Queue<Vector2Int>();

    private Vector2Int bossRoomPosition;

    // Colliders que bloquean las puertas del Boss
    private List<Collider2D> bossDoorColliders =
        new List<Collider2D>();

    // Para no revisar/desbloquear varias veces
    private bool bossUnlocked = false;


    IEnumerator Start()
    {
        totalRooms = baseRooms;

        Debug.Log(
            $"Generando nivel con {totalRooms} habitaciones."
        );

        yield return StartCoroutine(GenerateDungeon());
    }


    IEnumerator GenerateDungeon()
    {
        roomPositions.Clear();
        roomQueue.Clear();
        roomObjects.Clear();
        bossDoorColliders.Clear();

        bossUnlocked = false;

        Vector2Int startPos = Vector2Int.zero;

        roomPositions.Add(startPos);
        roomQueue.Enqueue(startPos);

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        List<Vector2Int> createdRooms =
            new List<Vector2Int> { startPos };


        // ==========================================
        // GENERAR POSICIONES DE LAS HABITACIONES
        // ==========================================

        while (roomPositions.Count < totalRooms)
        {
            if (roomQueue.Count == 0)
            {
                Vector2Int randomCreated =
                    createdRooms[
                        Random.Range(0, createdRooms.Count)
                    ];

                roomQueue.Enqueue(randomCreated);
            }

            Vector2Int current =
                roomQueue.Dequeue();

            ShuffleDirections(directions);

            foreach (Vector2Int dir in directions)
            {
                if (roomPositions.Count >= totalRooms)
                    break;

                Vector2Int nextPos =
                    current + dir;

                if (!roomPositions.Contains(nextPos))
                {
                    if (
                        Random.value <= expansionChance ||
                        roomQueue.Count == 0
                    )
                    {
                        roomPositions.Add(nextPos);
                        roomQueue.Enqueue(nextPos);
                        createdRooms.Add(nextPos);
                    }
                }
            }
        }


        // ==========================================
        // BUSCAR HABITACIÓN DEL BOSS
        // ==========================================

        Vector2Int bossPos = startPos;
        int maxDistance = 0;

        foreach (Vector2Int pos in createdRooms)
        {
            int distance =
                Mathf.Abs(pos.x - startPos.x) +
                Mathf.Abs(pos.y - startPos.y);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                bossPos = pos;
            }
        }

        bossRoomPosition = bossPos;

        Debug.Log(
            "Habitación Boss: " + bossRoomPosition
        );


        // ==========================================
        // INSTANCIAR HABITACIONES
        // ==========================================

        List<GameObject> availableRooms =
            new List<GameObject>(roomPrefabs);

        foreach (Vector2Int pos in createdRooms)
        {
            if (pos == startPos)
            {
                InstantiateRoom(
                    pos,
                    startRoomPrefab
                );
            }
            else if (pos == bossPos)
            {
                InstantiateRoom(
                    pos,
                    bossRoomPrefab
                );
            }
            else
            {
                if (availableRooms.Count > 0)
                {
                    int randomIndex =
                        Random.Range(
                            0,
                            availableRooms.Count
                        );

                    GameObject selectedRoom =
                        availableRooms[randomIndex];

                    InstantiateRoom(
                        pos,
                        selectedRoom
                    );

                    availableRooms.RemoveAt(
                        randomIndex
                    );
                }
                else
                {
                    GameObject fallbackPrefab =
                        roomPrefabs[
                            Random.Range(
                                0,
                                roomPrefabs.Length
                            )
                        ];

                    InstantiateRoom(
                        pos,
                        fallbackPrefab
                    );
                }
            }
        }


        // ==========================================
        // ESPERAR A QUE TODO ESTÉ CREADO
        // ==========================================

        yield return new WaitForFixedUpdate();
        yield return null;
        yield return null;

        Physics2D.SyncTransforms();

        navMeshSurface.BuildNavMeshAsync();


        // ==========================================
        // POSICIÓN INICIAL DEL JUGADOR
        // ==========================================

        Vector3 playerPosition =
            new Vector3(
                startPos.x * roomDistance,
                startPos.y * roomDistance,
                0f
            );
    }


    // ==================================================
    // CREAR UNA HABITACIÓN
    // ==================================================

    void InstantiateRoom(
        Vector2Int gridPos,
        GameObject prefab
    )
    {
        Vector3 worldPosition =
            new Vector3(
                gridPos.x * roomDistance,
                gridPos.y * roomDistance,
                0f
            );

        GameObject room =
            Instantiate(
                prefab,
                worldPosition,
                Quaternion.identity,
                transform
            );

        roomObjects.Add(
            gridPos,
            room
        );


        // ==========================================
        // BUSCAR PUERTAS
        // ==========================================

        Transform doorUp =
            room.transform.Find(
                "Building/Door_Up"
            );

        Transform doorDown =
            room.transform.Find(
                "Building/Door_Down"
            );

        Transform doorLeft =
            room.transform.Find(
                "Building/Door_Left"
            );

        Transform doorRight =
            room.transform.Find(
                "Building/Door_Right"
            );


        // ==========================================
        // BUSCAR PAREDES
        // ==========================================

        Transform wallUp =
            room.transform.Find(
                "Building/Wall_Up"
            );

        Transform wallDown =
            room.transform.Find(
                "Building/Wall_Down"
            );

        Transform wallLeft =
            room.transform.Find(
                "Building/Wall_Left"
            );

        Transform wallRight =
            room.transform.Find(
                "Building/Wall_Right"
            );


        // ==========================================
        // COMPROBAR HABITACIONES VECINAS
        // ==========================================

        bool hasUp =
            roomPositions.Contains(
                gridPos + Vector2Int.up
            );

        bool hasDown =
            roomPositions.Contains(
                gridPos + Vector2Int.down
            );

        bool hasLeft =
            roomPositions.Contains(
                gridPos + Vector2Int.left
            );

        bool hasRight =
            roomPositions.Contains(
                gridPos + Vector2Int.right
            );


        // ==========================================
        // PUERTAS / PAREDES
        // ==========================================

        doorUp.gameObject.SetActive(hasUp);
        wallUp.gameObject.SetActive(!hasUp);

        doorDown.gameObject.SetActive(hasDown);
        wallDown.gameObject.SetActive(!hasDown);

        doorLeft.gameObject.SetActive(hasLeft);
        wallLeft.gameObject.SetActive(!hasLeft);

        doorRight.gameObject.SetActive(hasRight);
        wallRight.gameObject.SetActive(!hasRight);


        // ==========================================
        // SI ESTA HABITACIÓN ESTÁ JUNTO AL BOSS
        // BUSCAR LA PUERTA QUE LLEVA AL BOSS
        // ==========================================

        SetupBossDoor(
            gridPos,
            room
        );
    }


    // ==================================================
    // CONFIGURAR LA PUERTA DEL BOSS
    // ==================================================

    void SetupBossDoor(
        Vector2Int roomPosition,
        GameObject room
    )
    {
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int direction in directions)
        {
            // Posición de la habitación vecina
            Vector2Int targetRoom =
                roomPosition + direction;


            // ¿Esa habitación es el Boss?
            if (targetRoom != bossRoomPosition)
                continue;


            Transform door = null;


            if (direction == Vector2Int.up)
            {
                door =
                    room.transform.Find(
                        "Building/Door_Up"
                    );
            }
            else if (direction == Vector2Int.down)
            {
                door =
                    room.transform.Find(
                        "Building/Door_Down"
                    );
            }
            else if (direction == Vector2Int.left)
            {
                door =
                    room.transform.Find(
                        "Building/Door_Left"
                    );
            }
            else if (direction == Vector2Int.right)
            {
                door =
                    room.transform.Find(
                        "Building/Door_Right"
                    );
            }


            if (door == null)
            {
                Debug.LogError(
                    "No se encontró la puerta que lleva al Boss."
                );

                continue;
            }


            // Buscar LockedCollider
            Transform lockedCollider =
                door.Find(
                    "LockedCollider"
                );


            if (lockedCollider == null)
            {
                Debug.LogError(
                    "La puerta " +
                    door.name +
                    " no tiene un hijo llamado LockedCollider."
                );

                continue;
            }


            Collider2D collider =
                lockedCollider.GetComponent<Collider2D>();


            if (collider == null)
            {
                Debug.LogError(
                    "LockedCollider no tiene Collider2D."
                );

                continue;
            }


            // Activar bloqueo
            collider.enabled = true;


            // Guardarlo para desbloquearlo después
            bossDoorColliders.Add(
                collider
            );


            Debug.Log(
                "PUERTA DEL BOSS BLOQUEADA en habitación " +
                roomPosition
            );
        }
    }


    // ==================================================
    // COMPROBAR ENEMIGOS DE TODO EL MAPA
    // ==================================================

    public bool AreAllEnemiesDefeated()
    {
        foreach (
            GameObject room
            in roomObjects.Values
        )
        {
            EnemyHealth[] enemies =
                room.GetComponentsInChildren<EnemyHealth>();


            if (enemies.Length > 0)
            {
                return false;
            }
        }


        return true;
    }


    // ==================================================
    // ACTUALIZAR
    // ==================================================

    void Update()
    {
        // Ya se desbloqueó
        if (bossUnlocked)
            return;


        // Todavía no hay puerta configurada
        if (bossDoorColliders.Count == 0)
            return;


        // ¿Ya no queda ningún enemigo?
        if (AreAllEnemiesDefeated())
        {
            bossUnlocked = true;


            foreach (
                Collider2D collider
                in bossDoorColliders
            )
            {
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }


            Debug.Log(
                "TODOS LOS ENEMIGOS DERROTADOS. " +
                "PUERTA DEL BOSS ABIERTA."
            );
        }
    }


    // ==================================================
    // MEZCLAR DIRECCIONES
    // ==================================================

    void ShuffleDirections(
        Vector2Int[] array
    )
    {
        for (int i = 0; i < array.Length; i++)
        {
            Vector2Int temp =
                array[i];

            int randomIndex =
                Random.Range(
                    i,
                    array.Length
                );

            array[i] =
                array[randomIndex];

            array[randomIndex] =
                temp;
        }
    }


    // ==================================================
    // MÉTODOS PÚBLICOS
    // ==================================================

    public bool RoomExists(
        Vector2Int position
    )
    {
        return roomPositions.Contains(
            position
        );
    }


    public void SetCurrentRoomVisible(
        Vector2Int currentRoom
    )
    {
        foreach (
            KeyValuePair<Vector2Int, GameObject> room
            in roomObjects
        )
        {
            Transform blackSquare =
                room.Value.transform.Find(
                    "BlackSquare"
                );


            if (blackSquare == null)
                continue;


            blackSquare.gameObject.SetActive(
                room.Key != currentRoom
            );
        }
    }


    public Vector2Int GetStartRoom()
    {
        return Vector2Int.zero;
    }


    public Vector2Int GetBossRoom()
    {
        return bossRoomPosition;
    }


    public GameObject GetRoomObject(
        Vector2Int position
    )
    {
        if (
            roomObjects.TryGetValue(
                position,
                out GameObject room
            )
        )
        {
            return room;
        }


        return null;
    }
}