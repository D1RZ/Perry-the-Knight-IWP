using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Checkpoint currentCheckpoint;

    [SerializeField] private Room defaultRoom;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;

        Debug.Log(
            $"Checkpoint saved | Room: {checkpoint.GetRoom().name}"
        );
    }

    public Vector2 GetRespawnPosition()
    {
        return currentCheckpoint != null
            ? currentCheckpoint.GetSpawnPosition()
            : PlayerController.Instance.playerStartPosition;
    }

    public Room GetRespawnRoom()
    {
        return currentCheckpoint != null
            ? currentCheckpoint.GetRoom()
            : defaultRoom;
    }

}