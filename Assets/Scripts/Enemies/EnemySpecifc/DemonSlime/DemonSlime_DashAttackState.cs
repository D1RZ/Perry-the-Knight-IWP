using UnityEngine;

public class DemonSlime_DashAttackState : State
{
    private float dashWindup = 0.1f; // for exclaimation mark showing etc
    private float attackTimer;
    private bool hasStartedAttack = false;
    private bool hasStartedDash = false;
    private float dashTimer;
    public FlyingEnemyAttackState_Data attackStateData;
    private int currentDashCount;
    private int maxDashCount;

    public DemonSlime_DashAttackState(FlyingEnemyAttackState_Data attackStateData)
    {
        this.attackStateData = attackStateData;
    }

    public override void Enter(Entity entity)
    {
        entity.SetTarget(PlayerController.Instance.gameObject);
        entity.Anim.Play("Slime_Dash");
        entity.SetVelocity(0);
        entity.isChasing = true;
        dashWindup = 0.15f;
        attackTimer = Random.Range(attackStateData.minAttackWindup, attackStateData.maxAttackWindup);
        currentDashCount = 0;
        maxDashCount = Random.Range(1,2); // upper bound exclusive
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

        if (!hasStartedDash)
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
                entity.spriteRenderer.GetComponent<PauseAnimation>().GetExclaimationMark().SetActive(false);
                entity.Anim.speed = 1;
                entity.transform.GetChild(0).GetComponent<Knockback>().isDashing = true;
                Vector2 dir = (entity.GetTarget().transform.position - entity.transform.position).normalized;
                entity.rb.constraints = RigidbodyConstraints2D.None;
                entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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
    }

    private void EndAttack(Entity entity)
    {
        // Stop movement instantly
        entity.rb.velocity = Vector2.zero;
        entity.transform.GetChild(0).GetComponent<Knockback>().isDashing = false;
        currentDashCount++;

        if (currentDashCount < maxDashCount)
        {
            // Reset for next dash
            hasStartedAttack = false;
            hasStartedDash = false;

            dashWindup = 0.15f;
            attackTimer = 0;

            entity.CheckFacingDirectionBasedOnTargetPos();
            entity.Anim.Play("Slime_Dash");
            entity.spriteRenderer.GetComponent<PauseAnimationMeele>().SetFlyHit(false); // resets attack hitbox

            return;
        }

        entity.stateMachine.SetNextState("CHASE", entity);
    }

    public override void Exit(Entity entity)
    {
        entity.isChasing = false;
    }

}
