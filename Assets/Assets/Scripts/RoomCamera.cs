using System.Collections;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;

    [Header("Jugador")]
    public Transform player;

    [Header("Tamaño de la sala")]
    public float roomWidth = 12f;
    public float roomHeight = 12f;

    [Header("Movimiento del jugador")]
    public float playerMoveDistance = 2f;

    [Header("Transición")]
    public float transitionSpeed = 20f;

    private bool isMoving = false;

    private Vector2Int currentRoom = Vector2Int.zero;

    private Player playerScript;

    void Start()
    {
        transform.position = new Vector3(0f, 0f, -10f);

        if (player != null)
        {
            playerScript = player.GetComponent<Player>();
        }
    }

    public void MoveToRoom(Vector2Int direction)
    {
        if (isMoving)
            return;

        currentRoom += direction;

        Vector3 targetCameraPosition = new Vector3(
            currentRoom.x * roomWidth,
            currentRoom.y * roomHeight,
            -10f
        );

        Vector3 targetPlayerPosition = player.position + new Vector3(
            direction.x * playerMoveDistance,
            direction.y * playerMoveDistance,
            0f
        );

        StartCoroutine(MoveRoom(
            targetCameraPosition,
            targetPlayerPosition
        ));
    }

    IEnumerator MoveRoom(
        Vector3 targetCameraPosition,
        Vector3 targetPlayerPosition
    )
    {
        isMoving = true;

        // Congelar controles del jugador
        if (playerScript != null)
        {
            playerScript.SetMovementEnabled(false);
        }

        Vector3 startCameraPosition = transform.position;
        Vector3 startPlayerPosition = player.position;

        float duration = 1f / transitionSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            // Cámara: exactamente 12 unidades
            transform.position = Vector3.Lerp(
                startCameraPosition,
                targetCameraPosition,
                t
            );

            // Jugador: exactamente 2 unidades
            player.position = Vector3.Lerp(
                startPlayerPosition,
                targetPlayerPosition,
                t
            );

            yield return null;
        }

        transform.position = targetCameraPosition;
        player.position = targetPlayerPosition;

        // Volver a permitir movimiento
        if (playerScript != null)
        {
            playerScript.SetMovementEnabled(true);
        }

        isMoving = false;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public Vector2Int GetCurrentRoom()
    {
        return currentRoom;
    }
}
