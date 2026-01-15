using System;
using UnityEngine;

public class Enemy : Entity
{
    public float health;
    public GameObject HealthBar;
    public static Action<Enemy> OnEnemyHit; // static event shared by all enemies
    public static Action<Enemy> OnBossHit; // only for boss enemies
    public float healthBar1PercentWidth; // for ui manager
    public bool knockedUp = false;
    public float knockedUpGraceTimer = 0f;
    public string hitAnim;
    public bool isStunned = false;
    public float stunTime;
    public float stunTimer;
    public bool isBlocking = false;
    public Transform BlockParticlePos;
    public bool canKnockUp = true;
    public bool InCutscene = false;

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

        if (InCutscene) return;

        if (health <= 0)
        {
            DeadEvent();
            return;
        }

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
                rb.constraints = RigidbodyConstraints2D.None;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                Animator anim = spriteRenderer.GetComponent<Animator>();
                if (anim != null) anim.speed = 1;
                OnStunEnd();
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
                OnStunEnd();
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

    public virtual void DeadEvent()
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

    public virtual void SetHealth(float dmg)
    {
        health -= dmg;
        if (health <= 0f) health = 0f;
        HealthBar.SetActive(true);
        OnEnemyHit.Invoke(this);
    }

    public virtual void OnStunEnd()
    {
        CurrentState.Enter(this);
    }

}