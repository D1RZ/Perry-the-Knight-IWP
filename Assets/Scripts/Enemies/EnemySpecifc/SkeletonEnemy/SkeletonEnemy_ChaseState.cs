using UnityEngine;

public class SkeletonEnemy_ChaseState : State
{
    private ChaseState_Data skeletonChaseData;

    public SkeletonEnemy_ChaseState(ChaseState_Data stateData)
    {
        this.skeletonChaseData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Skeleton Enemy - Walk");
        entity.isChasing = true;
        float direction = Mathf.Sign(entity.GetTarget().transform.position.x - entity.transform.position.x);
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        entity.SetVelocity(direction * skeletonChaseData.ChaseMovementSpeed);
        entity.CheckFacingDirection();
    }

    public override void onUpdate(Entity entity)
    {
        float direction = Mathf.Sign(entity.GetTarget().transform.position.x - entity.transform.position.x);

        entity.SetVelocity(direction * skeletonChaseData.ChaseMovementSpeed);

        entity.CheckFacingDirection();

        if (PlayerController.Instance._PlayerData.HealthData > 0)
        {
            if (entity.CheckAttackTarget("Player") && Mathf.Abs(entity.transform.position.y - PlayerController.Instance.transform.position.y) < 1)
            {
                entity.stateMachine.SetNextState("ATTACK", entity);
                return;
            }

            if (Vector2.Distance(entity.GetTarget().gameObject.transform.position, entity.gameObject.transform.position) > skeletonChaseData.MaxChaseDistance
                || Mathf.Abs(entity.transform.position.y - PlayerController.Instance.transform.position.y) > 1)
            {
                entity.stateMachine.SetNextState("PATROL", entity);
                return;
            }
        }
    }

    public override void Exit(Entity entity)
    {
        entity.isChasing = false;
    }

}
