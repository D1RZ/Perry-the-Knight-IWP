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
    private bool isTransitioning;

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
        if (currentRoom != null)
        {
            currentRoom.OnExitRoom();
            currentRoom.gameObject.SetActive(false);
        }

        // 3? Switch room
        currentRoom = newRoom;
        currentRoom.gameObject.SetActive(true);

        PlayerController.Instance.SetCanMove(false);
        if(!respawn) PlayerController.Instance.transform.position = currentRoom.playerSpawnPos.position;

        // 4? Update camera bounds
        if (confiner != null && newRoom.cameraBounds != null)
        {
            confiner.m_BoundingShape2D = newRoom.cameraBounds;
            confiner.InvalidateCache();
        }

        currentRoom.OnEnterRoom();

        PlayerController.Instance.SetCanMove(true);
        if(respawn) PlayerController.Instance.Respawn(CheckpointManager.Instance.GetRespawnPosition());

        // 5? Fade back in
        if (useFade && FadeManager.Instance != null)
            yield return FadeManager.Instance.FadeIn();

        isTransitioning = false;
    }

    // Useful for respawn
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
}