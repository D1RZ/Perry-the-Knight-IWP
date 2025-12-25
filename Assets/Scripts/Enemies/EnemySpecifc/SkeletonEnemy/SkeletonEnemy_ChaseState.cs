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
        entity.Anim.SetBool("isWalking", true);
        entity.isChasing = true;
        float direction = Mathf.Sign(entity.GetTarget().transform.position.x - entity.transform.position.x);
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        entity.SetVelocity(direction * skeletonChaseData.ChaseMovementSpeed);
        entity.CheckFacingDirection();
    }

    public override void onUpdate(Entity entity)
    {
        if (entity.GetTarget() == null)
            return;

        float playerX = entity.GetTarget().transform.position.x;
        float enemyX = entity.transform.position.x;

        float directionToPlayer = Mathf.Sign(playerX - enemyX);

        // LEDGE CHECK
        if (entity.CheckLedge())
        {
            Debug.Log("CHECKED LEDGE:" + "DP: " + directionToPlayer + "FD: " + entity.facingDirection);

            // Case 1: Player is on the OTHER side of the enemy
            if (directionToPlayer == entity.facingDirection)
            {
                // Flip and keep chasing
                entity.Flip();
                entity.SetVelocity(entity.facingDirection * skeletonChaseData.ChaseMovementSpeed);
                return;
            }
            else
            {
                // Player is beyond the ledge then stop chase
                entity.Flip();
                entity.SetVelocity(entity.facingDirection * skeletonChaseData.ChaseMovementSpeed);
                entity.stateMachine.SetNextState("PATROL", entity);
                return;
            }
        }

        // NORMAL CHASE
        entity.SetVelocity(entity.facingDirection * skeletonChaseData.ChaseMovementSpeed);
        entity.CheckFacingDirection();

        // ATTACK CHECK
        if (PlayerController.Instance._PlayerData.HealthData > 0)
        {
            if (entity.CheckAttackTarget("Player") &&
                Mathf.Abs(entity.transform.position.y - PlayerController.Instance.transform.position.y) < 1)
            {
                entity.stateMachine.SetNextState("ATTACK", entity);
                return;
            }

            // TOO FAR then STOP CHASE
            if (Vector2.Distance(entity.GetTarget().transform.position, entity.transform.position)
                > skeletonChaseData.MaxChaseDistance)
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
