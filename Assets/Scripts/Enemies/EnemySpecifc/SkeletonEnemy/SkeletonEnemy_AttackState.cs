using UnityEngine;

public class SkeletonEnemy_AttackState : State
{
    private SpearEnemy_AttackState_Data skeletonAttackStateData;

    public SkeletonEnemy_AttackState(SpearEnemy_AttackState_Data stateData)
    {
        this.skeletonAttackStateData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.SetVelocity(0);
        entity.Anim.speed = 1.5f;
        entity.Anim.Play("Skeleton Enemy - Attack");
        entity.CheckFacingDirectionBasedOnTargetPos();
        entity.hasFinishedAttack = false;
    }

    public override void onUpdate(Entity entity)
    {
        //if (Vector2.Distance(entity.gameObject.transform.position, PlayerController.Instance.gameObject.transform.position) < 0.8f)
        //    PlayerController.Instance.defaultwalkspeed = 2.5f;

        if(entity.hasFinishedAttack)
        {
            entity.Anim.speed = 1; // reset after animation ends

            entity.CheckFacingDirectionBasedOnTargetPos();
            entity.stateMachine.SetNextState("GUARD", entity);
            return;
        }
    }

    public override void Exit(Entity entity)
    {
        
    }

}
