using UnityEngine;

public class SpearEnemy_PatrolState : MoveState
{
    private SpearEnemy_PatrolState_Data spearPatrolData;

    public SpearEnemy_PatrolState(SpearEnemy_PatrolState_Data stateData)
    {
        this.spearPatrolData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Spear Goblin - Walk");
        entity.SetVelocity(spearPatrolData.patrolMovementSpeed);
    }

    public override void onUpdate(Entity entity)
    {
        if (PlayerController.Instance._PlayerData.HealthData > 0)
        {
            if (entity.CheckAttackTarget("Player") && Mathf.Abs(entity.transform.position.y - PlayerController.Instance.transform.position.y) < 1)
            {
                entity.stateMachine.SetNextState("ATTACK1", entity);
                return;
            }

            if (entity.CheckChaseTarget("Player"))
            {
                entity.stateMachine.SetNextState("CHASE", entity);
                return;
            }
        }

        entity.CheckWall();

        entity.SetVelocity(entity.GetCurrentVelocity());
    }
    
    public override void Exit(Entity entity)
    {
        
    }
}
