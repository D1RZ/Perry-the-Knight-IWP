//using Pathfinding;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class Enemy2 : Entity
//{
//    [SerializeField]
//    private D_MoveState moveStateData;

//    [SerializeField]
//    private D_IdleState idleStateData;

//    [SerializeField]
//    private List<Transform> Waypoints;

//    [SerializeField]
//    private Enemy2Chase enemyChase;

//    [SerializeField]
//    private GameObject DeathParticles;

//    float startTime;

//    private State currentState;

//    private float health;

//    public float Health
//    {
//        get
//        {
//            return health;
//        }

//        set
//        {
//            health = value;
//        }
//    }

//    private Vector2 Direction;

//    private Vector3 targetWaypoint;

//    bool InPatrol = false;  // currently moving to a waypoint

//    bool backTraversal = false;

//    bool EnterChase = false;

//    public enum State
//    {
//        EnterIdle,
//        IdleUpdate,
//        ExitIdle,
//        Move,
//        Chase,
//        Attack,
//        Death
//    }

//    public override void Start()
//    {
//        base.Start();

//        currentState = State.EnterIdle;

//        health = entityData.MaxHealth;

//        transform.position = Waypoints[0].position;
//    }
    
//    public void Update()
//    {
//        if (health <= 0)
//        {
//            currentState = State.Death;
//        }

//        switch (currentState)
//        {
//            case State.EnterIdle:

//                Anim.SetBool("Flight", true);

//                startTime = Time.time;

//                currentState = State.IdleUpdate;

//                break;

//            case State.IdleUpdate:

//                if (Time.time >= startTime + idleStateData.maxIdleTime)
//                {
//                    currentState = State.ExitIdle;
//                }

//                ///
//                if (CheckForPlayer() && currentState != State.Attack)
//                    currentState = State.Chase;

//                if (Vector2.Distance(Player.transform.position, transform.position) <= 0.8f)
//                    currentState = State.Attack;
//                ///
//                break;

//            case State.ExitIdle:

//                currentState = State.Move;

//                break;

//            case State.Move:

//                if (!CheckForPlayer())
//                {
//                   CheckWall();

//                   // front traversal through waypoints
//                   if (!backTraversal)
//                   {
//                       for (int i = 0; i < Waypoints.Count; i++)
//                       {
//                           if (!InPatrol)
//                           {
//                               if (Vector2.Distance(transform.position, Waypoints[i].position) >= 0.1f)
//                               {
//                                   targetWaypoint = Waypoints[i].transform.position;
                   
//                                   Direction = (Waypoints[i].position - transform.position).normalized;
                   
//                                   rb.velocity = Direction * moveStateData.movementSpeed;
                   
//                                   InPatrol = true;
//                               }
//                           }
//                       }
//                   }

//                  // back traversal through waypoints
//                  if (backTraversal)
//                  {
//                      for (int i = Waypoints.Count - 1; i >= 0; i--)
//                      {
//                        if (!InPatrol)
//                        {
//                           if (Vector2.Distance(transform.position, Waypoints[i].position) >= 0.1f)
//                           {
//                             targetWaypoint = Waypoints[i].transform.position;
                             
//                             Direction = (Waypoints[i].position - transform.position).normalized;
                             
//                             rb.velocity = Direction * moveStateData.movementSpeed;
                             
//                             InPatrol = true;
//                           }
//                        }
//                      }
//                   }

//                   if (Vector2.Distance(transform.position,targetWaypoint) <= 0.1f)
//                   {
//                       InPatrol = false;

//                        if (Vector2.Distance(transform.position, Waypoints[Waypoints.Count - 1].position) <= 0.1f)
//                        {
//                            Flip();
//                            backTraversal = true;
//                        }
//                        else if (Vector2.Distance(transform.position, Waypoints[0].position) <= 0.1f && transform.localScale.x == -1)
//                       {
//                            Flip();
//                            backTraversal = false;
//                       }
//                   }
//                }
//                else
//                {
//                    if (Vector2.Distance(Player.transform.position, transform.position) <= 0.8f)
//                        currentState = State.Attack;
//                    else
//                    currentState = State.Chase;
//                }

//                //CheckLedge();

//            break;

//           case State.Chase:

//           if (!EnterChase)
//           {
//               enemyChase.Start();
//               EnterChase = true;
//           }

//           enemyChase.Update();

//           if(Vector2.Distance(transform.position,Player.transform.position) <= 0.3f)
//           {
//             EnterChase = false;
//             currentState = State.Attack;
//           }

//           break;

//            case State.Attack:

//            Debug.Log("ATTACK");

//            CheckFacingDirectionWhenAttacking();

//            Anim.SetBool("Flight", false);
//            Anim.SetBool("Hit", false);
//            Anim.SetBool("Attack", true);

            

//           if (Vector2.Distance(transform.position, Player.transform.position) > 0.5f)
//           {
//                Anim.SetBool("Attack", false);
//                Anim.SetBool("Flight", true);
//                currentState = State.Chase;
//           }

//           break;

//           case State.Death:
           
//           Anim.SetBool("Hit", false);
//           Anim.SetBool("Attack", false);
//           Anim.SetBool("Flight", true);
//           Anim.SetBool("Death", true);

//           break;

//           default:

//           break;
//        }
//    }

//    bool CheckForPlayer()
//    {
//        return Vector2.Distance(Player.transform.position, transform.position) <= 5.0f;
//    }

//    void CheckFacingDirectionWhenAttacking()
//    {
//        if (rb.velocity.x >= 0.01f)
//            transform.localScale = new Vector3(1, 1, 1);
//        else if (rb.velocity.x <= -0.01f)
//            transform.localScale = new Vector3(-1, 1, 1);
//    }

//    void Hide()
//    {
//        if (Physics2D.OverlapCircle(transform.position, 1.5f, LayerMask.GetMask("Player")))
//            _PlayerController._PlayerData.HealthData -= 20;
//    }

//    void Die()
//    {
//        Instantiate(DeathParticles, transform.position, Quaternion.identity);
//        gameObject.SetActive(false);
//    }
//}
