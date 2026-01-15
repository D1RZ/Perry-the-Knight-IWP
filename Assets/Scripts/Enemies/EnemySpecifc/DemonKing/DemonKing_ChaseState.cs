using UnityEngine;

public class DemonKing_ChaseState : State
{
    private ChaseState_Data DemonKingChaseData;
    private bool resetChase = false; // after getting stunned
    
    // --- Decision thresholds ---
    private float slashRange = 4.5f;
    private float magicMinRange = 10f;

    // --- Cooldowns ---
    private float summonCooldown = 8f;
    private float summonTimer = 0;

    public DemonKing_ChaseState(ChaseState_Data stateData)
    {
        this.DemonKingChaseData = stateData;
    }

    public override void Enter(Entity entity)
    {
        resetChase = false;
        entity.Anim.Play("Demon_Walk", 0, 0f);
        entity.SetTarget(PlayerController.Instance.gameObject);
        entity.isChasing = true;
        float direction = Mathf.Sign(entity.GetTarget().transform.position.x - entity.transform.position.x);
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        entity.SetVelocity(direction * DemonKingChaseData.ChaseMovementSpeed);
        entity.CheckFacingDirection();
        Debug.Log("CHASE ENTER DEMON!");
    }

    public override void onUpdate(Entity entity)
    {
        entity.rb.constraints = RigidbodyConstraints2D.None;
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // --- Distance checks ---
        float distanceToPlayer = Vector2.Distance(
            entity.transform.position,
            entity.GetTarget().transform.position
        );

        summonTimer += Time.deltaTime;

        //if (summonTimer >= summonCooldown)
        //{
        //    summonTimer = 0;
        //    entity.stateMachine.SetNextState("SUMMON", entity);
        //    return;
        //}

        if (distanceToPlayer >= slashRange && distanceToPlayer <= magicMinRange)
        {
            entity.stateMachine.SetNextState("MAGIC", entity);
            return;
        }

        if (distanceToPlayer <= slashRange)
        {
            entity.stateMachine.SetNextState("SLASH", entity);
            return;
        }

        if (entity.GetTarget() != null) entity.CheckFacingDirectionBasedOnTargetPos();
        entity.SetVelocity(entity.facingDirection * DemonKingChaseData.ChaseMovementSpeed);
    }

    public override void Exit(Entity entity)
    {
        entity.isChasing = false;
    }

}