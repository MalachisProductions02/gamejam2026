using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [Header("Referencias")]
    public DungeonGenerator dungeonGenerator;
    public RoomCamera roomCamera;
    public GameObject roomUIPrefab;
    public RectTransform mapWindow;

    [Header("Configuración del mapa")]
    public float roomUISize = 50f;
    public float roomUISpacing = 5f;

    [Header("Colores")]
    public Color currentColor = Color.white;
    public Color visitedColor = new Color(0.35f, 0.35f, 0.35f);
    public Color discoveredColor = new Color(0.7f, 0.7f, 0.7f);

    private Dictionary<Vector2Int, GameObject> roomUIObjects =
        new Dictionary<Vector2Int, GameObject>();

    private HashSet<Vector2Int> visitedRooms =
        new HashSet<Vector2Int>();

        private HashSet<Vector2Int> discoveredRooms =
    new HashSet<Vector2Int>();

    private Vector2Int lastRoom;

    void Start()
    {
        StartCoroutine(InitializeMap());
    }

    IEnumerator InitializeMap()
    {
        // Esperar a que DungeonGenerator termine de generar las habitaciones
        yield return null;

        CreateMap();

        // Obtener la habitación inicial
        lastRoom = roomCamera.GetCurrentRoom();

        // Actualizar el mapa por primera vez
        UpdateMap();
    }

    void Update()
    {
        // Comprobar si el jugador cambió de habitación
        Vector2Int currentRoom = roomCamera.GetCurrentRoom();

        if (currentRoom != lastRoom)
        {
            lastRoom = currentRoom;

            UpdateMap();
        }
    }

    void CreateMap()
    {
        for (int x = -20; x <= 20; x++)
        {
            for (int y = -20; y <= 20; y++)
            {
                Vector2Int position = new Vector2Int(x, y);

                if (!dungeonGenerator.RoomExists(position))
                    continue;

                CreateRoomUI(position);
            }
        }
    }

    void CreateRoomUI(Vector2Int gridPosition)
    {
        GameObject roomUI = Instantiate(
            roomUIPrefab,
            transform
        );

        RectTransform rect = roomUI.GetComponent<RectTransform>();

        float distance = roomUISize + roomUISpacing;

        rect.anchoredPosition = new Vector2(
            gridPosition.x * distance,
            gridPosition.y * distance
        );

        // Todas empiezan invisibles
        roomUI.SetActive(false);

        roomUIObjects.Add(gridPosition, roomUI);
    }

    void UpdateMap()
    {
        Vector2Int currentRoom = roomCamera.GetCurrentRoom();

        // La habitación actual queda descubierta y visitada
        visitedRooms.Add(currentRoom);
        discoveredRooms.Add(currentRoom);

        // Descubrir las habitaciones que están alrededor
        Vector2Int[] directions =
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbor = currentRoom + direction;

            if (dungeonGenerator.RoomExists(neighbor))
            {
                discoveredRooms.Add(neighbor);
            }
        }

        // Actualizar la apariencia de todas las habitaciones
        foreach (KeyValuePair<Vector2Int, GameObject> room in roomUIObjects)
        {
            Vector2Int position = room.Key;
            GameObject roomObject = room.Value;

            Image image = roomObject.GetComponent<Image>();

            // HABITACIÓN ACTUAL
            if (position == currentRoom)
            {
                roomObject.SetActive(true);

                if (image != null)
                {
                    image.color = currentColor;
                }
            }
            // HABITACIÓN YA VISITADA
            else if (visitedRooms.Contains(position))
            {
                roomObject.SetActive(true);

                if (image != null)
                {
                    image.color = visitedColor;
                }
            }
            // HABITACIÓN DESCUBIERTA PERO NO VISITADA
            else if (discoveredRooms.Contains(position))
            {
                roomObject.SetActive(true);

                if (image != null)
                {
                    image.color = discoveredColor;
                }
            }
            // HABITACIÓN DESCONOCIDA
            else
            {
                roomObject.SetActive(false);
            }
        }
        MoveMapContent();
    }
    void MoveMapContent()
    {
        Vector2Int currentRoom = roomCamera.GetCurrentRoom();

        float distance = roomUISize + roomUISpacing;

        Vector2 targetPosition = new Vector2(
            -currentRoom.x * distance,
            -currentRoom.y * distance
        );

        RectTransform mapRect = GetComponent<RectTransform>();

        mapRect.anchoredPosition = targetPosition;
    }
}