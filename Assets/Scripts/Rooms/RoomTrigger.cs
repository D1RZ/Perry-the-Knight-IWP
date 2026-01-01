using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private Room targetRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        RoomManager.Instance.EnterRoom(targetRoom, false);
    }
}
