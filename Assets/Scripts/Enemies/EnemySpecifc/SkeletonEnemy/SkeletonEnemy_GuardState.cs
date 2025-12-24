using UnityEngine;

public class SkeletonEnemy_GuardState : State
{
    private GuardState_Data skeletonGuardStateData;

    private float guardTime;

    public SkeletonEnemy_GuardState(GuardState_Data stateData)
    {
        this.skeletonGuardStateData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Skeleton Enemy - Guard");
        entity.SetVelocity(0);
        entity.CheckFacingDirectionBasedOnTargetPos();
        var enemy = entity as Enemy;
        enemy.isBlocking = true;
        SetRandomGuardTime();
    }

    public override void onUpdate(Entity entity)
    {
        entity.CheckFacingDirectionBasedOnTargetPos();

        if(guardTime > 0)
        {
            guardTime -= Time.deltaTime;
        }
        else
        {
            if (entity.CheckAttackTarget("Player") && Mathf.Abs(entity.transform.position.y - PlayerController.Instance.transform.position.y) < 1)
            {
                entity.stateMachine.SetNextState("ATTACK", entity);
                return;
            }

            entity.stateMachine.SetNextState("CHASE", entity);
            return;
        }
    }
    
    public override void Exit(Entity entity)
    {
        var enemy = entity as Enemy;
        enemy.isBlocking = false;
    }

    protected void SetRandomGuardTime()
    {
        guardTime = Random.Range(skeletonGuardStateData.minGuardTime, skeletonGuardStateData.maxGuardTime);
    }

}
