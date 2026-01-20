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
        // Hard stop if input is basically zero
        if (Mathf.Abs(vX) < 0.01f)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(vX, rb.velocity.y);
        }
    }

    public void MoveVertical(float vY)
    {
        rb.velocity = new Vector2(rb.velocity.x,vY);
    }
}
