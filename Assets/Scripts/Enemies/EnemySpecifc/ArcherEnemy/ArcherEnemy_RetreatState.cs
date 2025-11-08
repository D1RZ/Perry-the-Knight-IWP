using UnityEngine;

public class ArcherEnemy_RetreatState : MoveState
{
    private ArcherEnemy_RetreatState_Data retreatData;
    private float retreatTimer;
    private bool hasAppliedForce;
    private bool hasLanded;

    public ArcherEnemy_RetreatState(ArcherEnemy_RetreatState_Data stateData)
    {
        this.retreatData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Archer Goblin - Idle"); // or “JumpBack” if you have that animation
        entity.SetVelocity(0);
        hasAppliedForce = false;
        hasLanded = false;
        retreatTimer = retreatData.retreatDuration;
    }

    public override void onUpdate(Entity entity)
    {
        ArcherEnemy archer = entity as ArcherEnemy;
        if (archer == null) return;

        // Apply the jump force once
        if (!hasAppliedForce)
        {
            hasAppliedForce = true;
            ApplyRetreatForce(archer);
        }

        // Check landing or timeout
        retreatTimer -= Time.deltaTime;

        if (!hasLanded && archer.CheckGrounded() && retreatTimer < retreatData.retreatDuration - 0.1f)
        {
            hasLanded = true;
        }

        if (retreatTimer <= 0f || hasLanded)
        {
            if (entity.CheckAttackTarget("Player") && Mathf.Abs(entity.transform.position.y - PlayerController.Instance.transform.position.y) < 1)
            {
                entity.stateMachine.SetNextState("ATTACK", entity);
            }
            else
            {
                entity.stateMachine.SetNextState("PATROL", entity);
            }
        }
    }

    private void ApplyRetreatForce(ArcherEnemy archer)
    {
        archer.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        archer.CheckFacingDirectionBasedOnTargetPos();

        if (Vector2.Distance(archer.transform.position, PlayerController.Instance.transform.position) < retreatData.minRetreatDistance)
        {
            Vector2 retreatForce = new Vector2(-archer.facingDirection * retreatData.jumpForce.x, retreatData.jumpForce.y);
            archer.rb.AddForce(retreatForce, ForceMode2D.Impulse);
            Debug.Log("RETREAT FORCE ADDED: " + retreatForce);
        }
    }

    public override void Exit(Entity entity)
    {
        entity.Anim.speed = 1f;
    }

}