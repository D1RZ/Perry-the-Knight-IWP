using UnityEngine;

public class ArcherEnemy_AttackState : MoveState
{
    private ArcherEnemy_AttackState_Data archerAttackData;

    private float attackCooldown;
    private float cooldownTimer;
    private bool hasStartedAttack;

    public ArcherEnemy_AttackState(ArcherEnemy_AttackState_Data stateData)
    {
        this.archerAttackData = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.SetVelocity(0);
        SetRandomAttackCooldownTime();
        hasStartedAttack = false;
        entity.Anim.Play("Archer Goblin - Idle");
        entity.CheckFacingDirectionBasedOnTargetPos();
    }

    public override void onUpdate(Entity entity)
    {
        ArcherEnemy archerEnemy = entity as ArcherEnemy;
        if (archerEnemy == null) return;

        float distanceToPlayer = Vector2.Distance(entity.transform.position, PlayerController.Instance.transform.position);

        // === TURN TOWARD PLAYER MID-ATTACK ===
        Vector2 directionToPlayer = PlayerController.Instance.transform.position - entity.transform.position;
        if (directionToPlayer.x > 0 && archerEnemy.facingDirection != 1)
        {
            archerEnemy.facingDirection = 1;
            archerEnemy.transform.localScale = new Vector3(1, 1, 1); // Flip sprite
        }
        else if (directionToPlayer.x < 0 && archerEnemy.facingDirection != -1)
        {
            archerEnemy.facingDirection = -1;
            archerEnemy.transform.localScale = new Vector3(-1, 1, 1); // Flip sprite
        }

        // === WAITING PHASE BEFORE ATTACK ===
        if (!hasStartedAttack)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f && entity.CheckGrounded())
            {
                StartAttack(entity);
            }

            return; // Don't check retreat until attack has finished
        }

        // === ATTACK ANIMATION HANDLING ===
        AnimatorStateInfo stateInfo = entity.Anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Archer Goblin - Attack"))
        {
            float normalizedTime = stateInfo.normalizedTime;

            if (normalizedTime >= 1f)
            {
                entity.Anim.speed = 1f; // Reset after attack

                // If lost target or target out of range
                if (!entity.CheckAttackTarget("Player") ||
                    Mathf.Abs(entity.transform.position.y - PlayerController.Instance.transform.position.y) > 1)
                {
                    entity.stateMachine.SetNextState("PATROL", entity);
                    return;
                }

                if (distanceToPlayer < archerEnemy.RetreatStateData.minRetreatDistance)
                {
                    entity.stateMachine.SetNextState("RETREAT", entity);
                    return;
                }
                else
                {
                    entity.stateMachine.ForceSetNextState("ATTACK", entity);
                    return;
                }
            }
        }
    }

    private void StartAttack(Entity entity)
    {
        hasStartedAttack = true;
        entity.Anim.Play("Archer Goblin - Attack", 0, 0f);
        entity.SetVelocity(0);
        entity.Anim.speed = 1f;
    }

    public override void Exit(Entity entity)
    {
        entity.Anim.speed = 1f;
        PlayerController.Instance.defaultwalkspeed = 5.5f;
    }

    protected void SetRandomAttackCooldownTime()
    {
        attackCooldown = Random.Range(archerAttackData.minAttackCooldown, archerAttackData.maxAttackCooldown);
        cooldownTimer = attackCooldown;
    }
}