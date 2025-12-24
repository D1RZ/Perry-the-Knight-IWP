using UnityEngine;

public class FlyingEnemy_AttackState : State
{
    private float attackTimer;
    private float dashWindup = 0.1f; // for exclaimation mark showing etc
    private bool hasStartedAttack = false;
    private bool hasStartedDash = false;
    private float dashTimer;
    public FlyingEnemyAttackState_Data attackStateData;
    
    public FlyingEnemy_AttackState(FlyingEnemyAttackState_Data attackStateData)
    {
        this.attackStateData = attackStateData;
    }

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Flying Enemy - Idle");
        entity.SetVelocity(0);
        entity.isChasing = true;
        dashWindup = 0.2f;
        attackTimer = Random.Range(attackStateData.minAttackWindup,attackStateData.maxAttackWindup);
        hasStartedAttack = false;
        hasStartedDash = false;
        entity.CheckFacingDirectionBasedOnTargetPos();
        entity.spriteRenderer.GetComponent<PauseAnimationMeele>().SetFlyHit(false); // resets attack hitbox
    }

    public override void onUpdate(Entity entity)
    {
        if (PlayerController.Instance._PlayerData.HealthData <= 0) return;

        if (!hasStartedAttack)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                StartAttack(entity);
            }
            return;
        }

        if(!hasStartedDash)
        {
            float diffX = entity.GetTarget().transform.position.x - entity.transform.position.x;
            if (Mathf.Abs(diffX) > 0.2f)
            {
                entity.CheckFacingDirectionBasedOnTargetPos();
            }
            entity.spriteRenderer.GetComponent<PauseAnimation>().GetExclaimationMark().SetActive(true);
            entity.Anim.speed = 0;
            entity.spriteRenderer.GetComponent<Collider2D>().enabled = false;
            dashWindup -= Time.deltaTime;
            if (dashWindup <= 0)
            {
                float diffX2 = entity.GetTarget().transform.position.x - entity.transform.position.x;
                if (Mathf.Abs(diffX2) > 0.2f)
                {
                    entity.CheckFacingDirectionBasedOnTargetPos();
                }
                hasStartedDash = true;
                entity.spriteRenderer.GetComponent<TrailRenderer>().enabled = true;
                entity.spriteRenderer.GetComponent<PauseAnimation>().GetExclaimationMark().SetActive(false);
                entity.Anim.speed = 1;
                Vector2 dir = (entity.GetTarget().transform.position - entity.transform.position).normalized;
                entity.SetVelocity(dir * attackStateData.dashSpeed);
                dashTimer = attackStateData.dashTime;
                entity.spriteRenderer.GetComponent<Collider2D>().enabled = true;
            }
            return;
        }

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            EndAttack(entity);
            return;
        }
    }
    
    private void StartAttack(Entity entity)
    {
        hasStartedAttack = true;
        float diffX = entity.GetTarget().transform.position.x - entity.transform.position.x;
        if (Mathf.Abs(diffX) > 0.2f)
        {
            entity.CheckFacingDirectionBasedOnTargetPos();
        }
        entity.Anim.Play("Flying Enemy - Attack", 0, 0f);
    }

    private void EndAttack(Entity entity)
    {
        // Stop movement instantly
        entity.rb.velocity = Vector2.zero;
        entity.stateMachine.SetNextState("COOLDOWN", entity);
    }

    public override void Exit(Entity entity)
    {
        entity.isChasing = false;
    }

}
