using System.Collections;
using UnityEngine;
using Cinemachine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Camera")]
    [SerializeField] private CinemachineConfiner2D confiner;

    [Header("Transition")]
    [SerializeField] private bool useFade = true;

    [SerializeField] private Room currentRoom;
    private Room OldRoom;
    private bool isTransitioning;

    public GameObject CurrentLeftBossBarrier;
    public GameObject CurrentRightBossBarrier;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Public entry point
    public void EnterRoom(Room newRoom, bool respawn)
    {
        if (isTransitioning) return;
        if (newRoom == currentRoom && !respawn) return;

        StartCoroutine(TransitionToRoom(newRoom, respawn));
    }

    private IEnumerator TransitionToRoom(Room newRoom,bool respawn)
    {
        isTransitioning = true;

        // 1? Fade to black
        if (useFade && FadeManager.Instance != null)
            yield return FadeManager.Instance.FadeOut();

        // 2? Exit old room
        if (currentRoom != null && OldRoom != newRoom)
        {
            OldRoom = currentRoom;
        }

        // 3? Switch room
        currentRoom = newRoom;
        currentRoom.gameObject.SetActive(true);

        currentRoom.OnEnterRoom();

        PlayerController.Instance.SetCanMove(false);
        if(!respawn) PlayerController.Instance.transform.position = currentRoom.playerSpawnPos.position;

        if (OldRoom != newRoom) OldRoom.OnExitRoom();

        // 4? Update camera bounds
        if (confiner != null && newRoom.cameraBounds != null)
        {
            confiner.m_BoundingShape2D = newRoom.cameraBounds;
            confiner.InvalidateCache();
        }

        PlayerController.Instance.SetCanMove(true);
        if(respawn) PlayerController.Instance.Respawn(CheckpointManager.Instance.GetRespawnPosition());

        // 5? Fade back in
        if (useFade && FadeManager.Instance != null)
            yield return FadeManager.Instance.FadeIn();

        if (respawn)
        {
            UIManager.Instance.respawnTrigger = false;
        }

        isTransitioning = false;
    }
    
    // Useful for respawn
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
}