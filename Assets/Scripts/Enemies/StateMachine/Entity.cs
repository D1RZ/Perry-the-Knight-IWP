using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int facingDirection;

    public FiniteStateMachine stateMachine; // stores current state,initialize state function 

    public D_Entity entityData; // stores max health,wall and ledge check distance etc

    public Rigidbody2D rb { get; private set; }

    public Animator Anim;

    [SerializeField]
    private Transform wallCheck;

    [SerializeField]
    private Transform ledgeCheck;

    [SerializeField]
    private Transform playerCheck;

    private Vector2 velocityWorkspace;

    private float velocityX = 0;

    protected State CurrentState; // stores the current fsm state of the entity

    protected State NextState; // stores the next fsm state the entity is going to transition to

    private IEnumerator currentCoroutine = null;

    protected GameObject target = null;  // stores the transform of target of entity

    public SpriteRenderer spriteRenderer;

    private Shader GUITextShader;

    private Shader NormalSpriteShader;

    public bool isChasing = false;

    public virtual void Start()
    {
        GUITextShader = Shader.Find("GUI/Text Shader");
        NormalSpriteShader = Shader.Find("Sprites/Default");
        facingDirection = 1;
        rb = GetComponent<Rigidbody2D>();

    }

    public void ChangeSpriteColor(bool turnWhite)
    {
        if(turnWhite)
        {
            spriteRenderer.material.shader = GUITextShader;
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.material.shader = NormalSpriteShader;
            spriteRenderer.color = Color.white;
        }
    }

    public virtual void SetVelocity(float velocity) // changes velocity of enemy according to their facing direction
    {
        velocityX = velocity;
        if (!isChasing) velocityWorkspace.Set(velocity * facingDirection, rb.velocity.y);
        else velocityWorkspace.Set(velocity,rb.velocity.y);
        rb.velocity = velocityWorkspace;
    }

    public virtual bool CheckWall()
    {
        if (Physics2D.Raycast(wallCheck.position,transform.right * facingDirection, entityData.wallCheckDistance, LayerMask.GetMask("Platforms", "Ledges")))
        {
            Flip();
        }

        return false;
    }

    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position,Vector2.down,entityData.ledgeCheckDistance, LayerMask.GetMask("Platforms", "Ledges"));
    }

    public virtual bool CheckChaseTarget(string layerOfTarget)
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, transform.right * facingDirection, entityData.chaseRaycastDistance, LayerMask.GetMask(layerOfTarget));

        if (hit.collider != null)
        {
            target = hit.transform.gameObject;
            return true;
        }

        target = null;
        return false;
    }
    
    public virtual bool CheckAttackTarget(string layerOfTarget)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(playerCheck.position,entityData.attackDetectionRadius, LayerMask.GetMask(layerOfTarget));

        if (hitCollider != null)
        {
            target = hitCollider.gameObject;
            return true;
        }

        return false;
    }    

    public virtual void Flip()
    {
        facingDirection *= -1;

        //velocityX *= -1;

        transform.localScale *= new Vector2(-1,1);
    }

    public void CheckFacingDirection()
    {
        if((facingDirection > 0 && velocityX < 0) || (facingDirection < 0 && velocityX > 0))
        {
            facingDirection *= -1;

            transform.localScale *= new Vector2(-1, 1);
        }
    }
    
    public void CheckFacingDirectionBasedOnTargetPos()
    {
        // is left of enemy
        if(target.transform.position.x < transform.position.x && facingDirection > 0)
        {
            facingDirection *= -1;

            transform.localScale *= new Vector2(-1, 1);
        }
        else if(target.transform.position.x > transform.position.x && facingDirection < 0)
        {
            facingDirection *= -1;

            transform.localScale *= new Vector2(-1, 1);
        }
    }

    public void SetCurrentState(State currentState)
    {
        CurrentState = currentState;
    }

    public State GetCurrentState()
    {
        return CurrentState;
    }

    public void SetNextState(State nextState)
    {
        NextState = nextState;
    }

    public State GetNextState()
    {
        return NextState;
    }

    // For entity to start coroutine from state since states arent mono behaviours
    public Coroutine StartCoroutineFromState(IEnumerator routine)
    {
        if (currentCoroutine != null) StopCurrentCoroutine();
        currentCoroutine = routine; // keeps a reference to enemy current coroutine
        return StartCoroutine(routine);
    }

    public void StopCurrentCoroutine()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = null;
    }

    public virtual void OnDrawGizmos()
    {
        if(wallCheck != null) Gizmos.DrawLine(wallCheck.position,wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public GameObject GetTarget()
    {
        return target;
    }

    public void SetFacingDirection(int newDirectionX)
    {
        facingDirection = newDirectionX;
    }

    public float GetCurrentVelocity()
    {
        return velocityX;
    }

}
