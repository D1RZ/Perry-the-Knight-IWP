using UnityEngine;

public class Room : MonoBehaviour
{
    public Collider2D cameraBounds;
    public Transform enemiesParent;
    public Transform playerSpawnPos;

    // for boss rooms
    public GameObject LeftBossBarrier;
    public GameObject RightBossBarrier; 

    public void OnEnterRoom()
    {
        if(LeftBossBarrier) RoomManager.Instance.CurrentLeftBossBarrier = LeftBossBarrier;
        if(RightBossBarrier) RoomManager.Instance.CurrentRightBossBarrier = RightBossBarrier;
        transform.gameObject.SetActive(true);
    }

    public void OnExitRoom()
    {
        transform.gameObject.SetActive(false);
    }

}
