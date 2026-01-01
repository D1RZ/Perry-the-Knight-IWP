using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float collectDistance = 0.15f;
    [SerializeField] private float pickupDelay = 0.25f;

    private Transform player;
    private Rigidbody2D parentRb;
    private bool canPickup = false;

    private void Awake()
    {
        parentRb = GetComponentInParent<Rigidbody2D>();
    }

    private void Start()
    {
        Invoke(nameof(EnablePickup), pickupDelay);
    }

    private void EnablePickup()
    {
        canPickup = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!canPickup || player != null) return;

        if (other.CompareTag("Player"))
            player = other.transform;
    }

    private void FixedUpdate()
    {
        if (!canPickup || player == null || parentRb == null) return;

        Vector2 toPlayer = (Vector2)player.position - parentRb.position;
        float distance = toPlayer.magnitude;

        if (distance <= collectDistance)
        {
            Collect();
            return;
        }

        Vector2 direction = toPlayer.normalized;

        // Clamp movement so we never overshoot
        float moveStep = Mathf.Min(moveSpeed * Time.fixedDeltaTime, distance);

        parentRb.MovePosition(parentRb.position + direction * moveStep);

        // Optional: rotate toward movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        parentRb.MoveRotation(angle);
    }

    private void Collect()
    {
        UIManager.Instance.UpdateHealthPotion();
        Destroy(parentRb.gameObject);
    }

}

