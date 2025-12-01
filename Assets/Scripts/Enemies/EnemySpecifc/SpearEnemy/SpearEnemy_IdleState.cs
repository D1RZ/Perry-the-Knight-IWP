using UnityEngine;

public class SpearEnemy_IdleState : IdleState
{
    private IdleState_Data spearIdleData;

    public SpearEnemy_IdleState(IdleState_Data stateData)
    {
        this.spearIdleData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Spear Goblin - Idle");
        SetRandomIdleTime();
        entity.SetVelocity(0);
    }

    public override void onUpdate(Entity entity)
    {
        if(!entity.CheckGrounded()) return; // if enemy is in air then dont update idle state (can serve as psuedo in air state)

        Animator anim = entity.spriteRenderer.GetComponent<Animator>();
        if (anim != null && anim.speed == 0) anim.speed = 1;

        if (idleTime > 0)
        {
            idleTime -= Time.deltaTime;
        }
        else
        {
            entity.stateMachine.SetNextState("PATROL", entity);
        }
    }

    public override void Exit(Entity entity)
    {

    }

    protected void SetRandomIdleTime()
    {
        idleTime = Random.Range(spearIdleData.minIdleTime,spearIdleData.maxIdleTime);
    }

}
