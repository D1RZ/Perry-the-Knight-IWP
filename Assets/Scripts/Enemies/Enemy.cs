using System;
using UnityEngine;

public class Enemy : Entity
{
    public float health { get; private set; }
    public GameObject HealthBar;
    public static Action<Enemy> OnEnemyHit; // static event shared by all enemies
    public float healthBar1PercentWidth; // for ui manager
    public bool knockedUp = false;
    public float knockedUpGraceTimer = 0f;
    public string hitAnim;
    public bool isStunned = false;
    public float stunTime;
    public float stunTimer;
    public bool isBlocking = false;
    public Transform BlockParticlePos;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        if (transform.localScale.x > 0) facingDirection = 1;
        else if (transform.localScale.x < 0) facingDirection = -1;
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
                Debug.Log("KNOCKED UP DONE!");
                knockedUp = false;
                if (isStunned) isStunned = false;
                rb.velocity = Vector3.zero; // prevent any form of sliding
                Animator anim = spriteRenderer.GetComponent<Animator>();
                if (anim != null) anim.speed = 1;
                CurrentState.Enter(this);
                rb.gravityScale = 1;
                Debug.Log("Enemy Landed");
            }
            return;
        }

        if (isStunned)
        {
            if (stunTimer > 0) stunTimer -= Time.deltaTime;
            else
            {
                isStunned = false;
                CurrentState.Enter(this);
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

    public void ResetEnemy()
    {
        health = entityData.MaxHealth;
        HealthBar.SetActive(false);

        knockedUp = false;
        isStunned = false;
        isBlocking = false;

        stunTimer = 0f;
        knockedUpGraceTimer = 0f;
    }

    public void SetHealth(float dmg)
    {
        health -= dmg;
        if (health <= 0) return;
        HealthBar.SetActive(true);
        OnEnemyHit.Invoke(this);
    }

}
