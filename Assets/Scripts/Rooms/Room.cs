using UnityEngine;

public class Room : MonoBehaviour
{
    public Collider2D cameraBounds;
    public Transform enemiesParent;
    public Transform playerSpawnPos;

    // for boss rooms
    [SerializeField] private bool isBossRoom = false;
    public GameObject LeftBossBarrier;
    public GameObject RightBossBarrier;
    public GameObject DemonSlime;
    public GameObject DemonKing;
    public GameObject CutsceneTrigger;
    
    public void OnEnterRoom()
    {
        if(LeftBossBarrier) RoomManager.Instance.CurrentLeftBossBarrier = LeftBossBarrier;
        if(RightBossBarrier) RoomManager.Instance.CurrentRightBossBarrier = RightBossBarrier;
        if (isBossRoom)
        {
            CameraManager.Instance.Follow(PlayerController.Instance.transform);
            CutsceneTrigger.GetComponent<Collider2D>().enabled = true;
            CutsceneTrigger.GetComponent<BossRoomCutscenes>().DialogueUI.SetActive(false);
            DemonSlime.GetComponent<DemonSlime>().health = DemonSlime.GetComponent<DemonSlime>().entityData.MaxHealth;
            DemonKing.GetComponent<DemonKing>().health = DemonKing.GetComponent<DemonKing>().entityData.MaxHealth;
            DemonSlime.GetComponent<DemonSlime>().HealthBar.SetActive(false);
            DemonSlime.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            DemonSlime.GetComponent<DemonSlime>().spriteRenderer.GetComponent<Animator>().Play("Slime_Idle", 0, 0.0f);
            DemonSlime.GetComponent<Collider2D>().enabled = true;
            DemonSlime.transform.GetChild(2).gameObject.SetActive(false);
            DemonKing.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            DemonKing.GetComponent<DemonKing>().HealthBar.SetActive(false);
            DemonKing.SetActive(false);
            DemonSlime.SetActive(false);
            LeftBossBarrier.SetActive(false);
            RightBossBarrier.SetActive(false);
        }
        transform.gameObject.SetActive(true);
    }

    public void OnExitRoom()
    {
        transform.gameObject.SetActive(false);
    }

}