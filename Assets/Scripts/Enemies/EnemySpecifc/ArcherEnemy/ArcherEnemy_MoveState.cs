using UnityEngine;

public class ArcherEnemy_PatrolState : MoveState
{
    private PatrolState_Data spearPatrolData;

    public ArcherEnemy_PatrolState(PatrolState_Data stateData)
    {
        this.spearPatrolData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        entity.Anim.Play("Archer Goblin - Walk");
        entity.SetVelocity(spearPatrolData.patrolMovementSpeed);
    }

    public override void onUpdate(Entity entity)
    {
        entity.CheckWall();

        entity.CheckLedge();

        entity.SetVelocity(entity.GetCurrentVelocity());

        if (PlayerController.Instance._PlayerData.HealthData > 0)
        {
            float distanceToPlayer = Vector2.Distance(entity.transform.position, PlayerController.Instance.transform.position);

            ArcherEnemy archerEnemy = entity as ArcherEnemy;
            if (archerEnemy == null) return;

            if (distanceToPlayer < archerEnemy.RetreatStateData.minRetreatDistance)
            {
                Debug.Log("SET TO RETREAT");
                // Too close then back off first
                entity.stateMachine.SetNextState("RETREAT", entity);
                return;
            }

            if (entity.CheckAttackTarget("Player") && Mathf.Abs(entity.transform.position.y - PlayerController.Instance.transform.position.y) < 1)
            {
                entity.stateMachine.SetNextState("ATTACK", entity);
                return;
            }
        }
    }
    
    public override void Exit(Entity entity)
    {
        
    }

}
