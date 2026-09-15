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

    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeMagnitude = 0.15f;

    private float shakeTime = 0f;
    private Vector3 baseCameraPosition;

    void Start()
    {
        baseCameraPosition = new Vector3(0f, 0f, -10f);
        transform.position = baseCameraPosition;

        if (player != null)
        {
            playerScript = player.GetComponent<Player>();
        }

        StartCoroutine(InitializeRoomVisibility());
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
        UpdateRoomVisibility();

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

            transform.position = Vector3.Lerp(
                startCameraPosition,
                targetCameraPosition,
                t
            );

            player.position = Vector3.Lerp(
                startPlayerPosition,
                targetPlayerPosition,
                t
            );

            yield return null;
        }

        baseCameraPosition = targetCameraPosition;
        transform.position = baseCameraPosition;
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
    private void UpdateRoomVisibility()
    {
        dungeonGenerator.SetCurrentRoomVisible(currentRoom);
    }

    IEnumerator InitializeRoomVisibility()
    {
        yield return null;

        dungeonGenerator.SetCurrentRoomVisible(currentRoom);
    }

    public void ShakeCamera()
    {
        shakeTime = shakeDuration;
    }

    void LateUpdate()
    {
        if (isMoving)
            return;

        if (shakeTime > 0f)
        {
            Vector3 shakeOffset =
                Random.insideUnitCircle * shakeMagnitude;

            transform.position = baseCameraPosition + shakeOffset;

            shakeTime -= Time.deltaTime;
        }
        else
        {
            transform.position = baseCameraPosition;
        }
    }
}
