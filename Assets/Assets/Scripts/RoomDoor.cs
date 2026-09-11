using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    public Vector2Int direction;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        RoomCamera roomCamera = Camera.main.GetComponent<RoomCamera>();

        if (roomCamera == null)
            return;

        if (roomCamera.IsMoving())
            return;

        Debug.Log("Jugador entró en una puerta: " + direction);

        roomCamera.MoveToRoom(direction);
    }
}