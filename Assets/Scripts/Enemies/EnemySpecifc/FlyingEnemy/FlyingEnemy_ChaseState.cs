using UnityEngine;

public class FlyingEnemy_ChaseState : ChaseState
{
    public ChaseState_Data chaseStateData;
    private float lastChaseDir = 1f;
    private float verticalSpeed;

    public FlyingEnemy_ChaseState(ChaseState_Data stateData)
    {
        chaseStateData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Flying Enemy - Idle");
        entity.isChasing = true;
        float diffX = entity.GetTarget().transform.position.x - entity.transform.position.x;
        float direction;
        // If enemy is directly above player, keep previous chase direction
        if (Mathf.Abs(diffX) < 0.2f)
        {
            direction = lastChaseDir;
        }
        else
        {
            direction = Mathf.Sign(diffX);
            lastChaseDir = direction; // store stable direction
        }
        float verticalSpeed = chaseStateData.VerticalCorrectionSpeed;
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        entity.SetVelocity(direction * chaseStateData.ChaseMovementSpeed);
        entity.CheckFacingDirection();
    }

    public override void onUpdate(Entity entity)
    {
        Debug.Log("IS CHASING!");

        if (PlayerController.Instance._PlayerData.HealthData <= 0) return;

        float diffX = entity.GetTarget().transform.position.x - entity.transform.position.x;
        float direction;

        // If enemy is directly above player, keep previous chase direction
        if (Mathf.Abs(diffX) < 0.2f)
        {
            direction = lastChaseDir;
        }
        else
        {
            direction = Mathf.Sign(diffX);
            lastChaseDir = direction; // store stable direction
        }

        float verticalDiff = entity.transform.position.y - entity.GetTarget().transform.position.y;
        float yDir = 0f;

        float verticalThreshold = 3; // tweak as needed

        if (verticalDiff > verticalThreshold)
        {
            // Enemy too high then move down
            yDir = -1f;
        }
        else if (verticalDiff < -verticalThreshold)
        {
            // Enemy too low then move up
            yDir = +1f;
        }

        // Apply velocity using combined direction
        Vector2 finalVel = new Vector2(
            direction * chaseStateData.ChaseMovementSpeed,
            yDir * chaseStateData.VerticalCorrectionSpeed
        );

        entity.SetVelocity(finalVel);

        entity.CheckFacingDirection();

        if (entity.CheckAttackTarget("Player"))
        {
            entity.stateMachine.SetNextState("ATTACK", entity);
            return;
        }

        if (Vector2.Distance(entity.GetTarget().gameObject.transform.position, entity.gameObject.transform.position) > chaseStateData.MaxChaseDistance)
        {
            entity.stateMachine.SetNextState("IDLE", entity);
            return;
        }
    }
    
    public override void Exit(Entity entity)
    {
        entity.isChasing = false;
    }

}
