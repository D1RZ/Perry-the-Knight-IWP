using UnityEngine;

public class SimpleTriggerTest : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Hitbox triggered with {collision.name}");
    }
}
