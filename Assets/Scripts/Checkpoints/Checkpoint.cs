using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Room owningRoom;
    [SerializeField] private Transform spawnPoint;

    private bool hasBeenActivated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (hasBeenActivated) return;

        hasBeenActivated = true;
        CheckpointManager.Instance.SetCheckpoint(this);
        Animator animator = transform.GetComponent<Animator>();
        animator.SetTrigger("Save");
        UIManager.Instance.AnimateHealthIncrease(PlayerController.Instance._PlayerData.MaxHealth + 50);
    }

    public Room GetRoom() => owningRoom;
    public Vector2 GetSpawnPosition() => spawnPoint.position;
}
