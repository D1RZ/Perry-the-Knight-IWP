using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveHorizontal(float vX)
    {
        rb.velocity = new Vector2(vX, rb.velocity.y);
        Debug.Log("VELOCITY X: " + rb.velocity.x);
    }

    public void MoveVertical(float vY)
    {
        rb.velocity = new Vector2(rb.velocity.x,vY);
    }
}
