using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy : Entity
{
    public float health { get; private set; }
    public GameObject HealthBar;
    public static Action<Enemy> OnEnemyHit; // static event shared by all enemies
    public float healthBar1PercentWidth; // for ui manager
    public bool knockedUp = false;
    public float knockedUpGraceTimer = 0f;
    public string hitAnim;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        health = entityData.MaxHealth;
        Debug.Log("Enemy Starting Health: " + health);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Debug.Log("Enemy Health: " + health);

        if (health < 0) return;

        if (knockedUp)
        {
            // Count down the grace period
            if (knockedUpGraceTimer > 0)
            {
                knockedUpGraceTimer -= Time.deltaTime;
                return; // don't check grounded yet
            }

            // Only check grounded AFTER the grace timer expires
            if (CheckGrounded())
            {
                knockedUp = false;
                rb.velocity = Vector3.zero; // prevent any form of sliding
                Animator anim = spriteRenderer.GetComponent<Animator>();
                if (anim != null) anim.speed = 1;
                CurrentState.Enter(this);
                rb.gravityScale = 1;
                Debug.Log("Enemy Landed");
            }
            return;
        }

        stateMachine.OnUpdate(Time.deltaTime, this); // updates enemy state machine
    }
    
    public virtual void AttackVFX(int VFXAttackNo)
    {

    }

    public virtual void HitConnected(int AttackNo)
    {

    }

    public void SetHealth(float dmg)
    {
        health -= dmg;
        if (health <= 0) return;
        HealthBar.SetActive(true);
        OnEnemyHit.Invoke(this);
    }

}
