using UnityEngine;

public class SpearEnemy_ChaseState : State
{
    private SpearEnemy_ChaseState_Data spearChaseData;

    public SpearEnemy_ChaseState(SpearEnemy_ChaseState_Data stateData)
    {
        this.spearChaseData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Spear Goblin - Chase");
        entity.isChasing = true;
        float direction = Mathf.Sign(entity.GetTarget().transform.position.x - entity.transform.position.x);
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        entity.SetVelocity(direction * spearChaseData.ChaseMovementSpeed);
        entity.CheckFacingDirection();
    }

    public override void onUpdate(Entity entity)
    {
        float direction = Mathf.Sign(entity.GetTarget().transform.position.x - entity.transform.position.x);

        entity.SetVelocity(direction * spearChaseData.ChaseMovementSpeed);

        entity.CheckFacingDirection();

        if (entity.CheckAttackTarget("Player"))
        {
            entity.stateMachine.SetNextState("ATTACK1",entity);
            return;
        }    

        if(Vector2.Distance(entity.GetTarget().gameObject.transform.position, entity.gameObject.transform.position) > spearChaseData.MaxChaseDistance)
        {
            entity.stateMachine.SetNextState("PATROL", entity);
            return;
        }
    }
    
    public override void Exit(Entity entity)
    {
        entity.isChasing = false;
    }
}
