using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Checkpoint currentCheckpoint;

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
            : Vector2.zero;
    }

    public Room GetRespawnRoom()
    {
        return currentCheckpoint != null
            ? currentCheckpoint.GetRoom()
            : null;
    }

}