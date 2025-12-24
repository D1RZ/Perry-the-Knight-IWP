using UnityEngine;

public class SkeletonEnemy_PatrolState : MoveState
{
    private PatrolState_Data skeletonPatrolData;

    public SkeletonEnemy_PatrolState(PatrolState_Data stateData)
    {
        this.skeletonPatrolData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Skeleton Enemy - Walk");
        entity.SetVelocity(skeletonPatrolData.patrolMovementSpeed);
    }

    public override void onUpdate(Entity entity)
    {
        if (PlayerController.Instance._PlayerData.HealthData > 0)
        {
            if (entity.CheckAttackTarget("Player") && Mathf.Abs(entity.transform.position.y - PlayerController.Instance.transform.position.y) < 1)
            {
                entity.stateMachine.SetNextState("ATTACK", entity);
                return;
            }

            if (entity.CheckGuardTarget("Player") && Mathf.Abs(entity.transform.position.y - PlayerController.Instance.transform.position.y) < 1)
            {
                entity.stateMachine.SetNextState("GUARD", entity);
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
