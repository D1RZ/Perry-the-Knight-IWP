using UnityEngine;
using Pathfinding;

public class Enemy2Chase : MonoBehaviour
{
    public Transform target;

    //[SerializeField] private D_MoveState movestateData;

    [SerializeField] private Transform Player;

    [SerializeField] private Transform EnemySprite;

    public float nextWaypointDistance = 3f;

    Path path;

    int currentWaypoint = 0;

    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    // Start is called before the first frame update
   public void Start()
    {
        seeker = GetComponent<Seeker>();

        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath",0f,0.5f);
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
        seeker.StartPath(rb.position, Player.transform.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (path == null)
            return;

        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = (Vector2)path.vectorPath[currentWaypoint] - rb.position;
        //Vector2 force = direction * movestateData.chaseSpeed;

        //rb.velocity = force;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if(rb.velocity.x >= 0.01f)
        {
            EnemySprite.localScale = new Vector3(0.75f,0.75f, 1f);
        }
        else if (rb.velocity.x <= -0.01f)
        {
            EnemySprite.localScale = new Vector3(-0.75f,0.75f, 1f);
        }
    }
}
