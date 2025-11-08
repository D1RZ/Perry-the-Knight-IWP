using UnityEngine;

public class ArcherEnemy_IdleState : IdleState
{
    private IdleState_Data IdleData;

    public ArcherEnemy_IdleState(IdleState_Data stateData)
    {
        this.IdleData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Archer Goblin - Idle");
        SetRandomIdleTime();
        entity.SetVelocity(0);
    }

    public override void onUpdate(Entity entity)
    {
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
        idleTime = Random.Range(IdleData.minIdleTime,IdleData.maxIdleTime);
    }

}
