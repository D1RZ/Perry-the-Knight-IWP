using System;
using UnityEngine;

public class Enemy : Entity
{
    public float health { get; private set; }
    public GameObject HealthBar;
    public static Action<Enemy> OnEnemyHit; // static event shared by all enemies
    public float healthBar1PercentWidth; // for ui manager

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
