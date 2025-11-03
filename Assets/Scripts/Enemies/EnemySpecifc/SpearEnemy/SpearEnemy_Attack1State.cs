using UnityEngine;

public class SpearEnemy_Attack1State : MoveState
{
    private SpearEnemy_Attack1State_Data spearAttack1Data;

    // Cooldown timer variables
    private float attackCooldown = 0.25f; // seconds before starting attack
    private float cooldownTimer;
    private bool hasStartedAttack;

    public SpearEnemy_Attack1State(SpearEnemy_Attack1State_Data stateData)
    {
        this.spearAttack1Data = stateData;
    }

    public override void Enter(Entity entity)
    {
        entity.SetVelocity(0);
        cooldownTimer = attackCooldown;
        hasStartedAttack = false;
        entity.Anim.Play("Spear Goblin - Idle");
        entity.CheckFacingDirectionBasedOnTargetPos();
    }
    
    public override void onUpdate(Entity entity)
    {
        Debug.Log("PE Distance: " + Vector2.Distance(entity.gameObject.transform.position, PlayerController.Instance.gameObject.transform.position));

        // If still waiting for cooldown
        if (!hasStartedAttack)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                StartAttack(entity);
            }
            return; // Skip the rest until attack starts
        }
        
        if(Vector2.Distance(entity.gameObject.transform.position,PlayerController.Instance.gameObject.transform.position) < 0.8f)
        PlayerController.Instance.defaultwalkspeed = 2.5f;

        // gets info about current animator controller state in this case its attack animation state
        AnimatorStateInfo stateInfo = entity.Anim.GetCurrentAnimatorStateInfo(0);

        // Wait for animation to finish before checking transitions
        if (stateInfo.IsName("Spear Goblin - Attack"))
        {
            float normalizedTime = stateInfo.normalizedTime; // 0 to 1+
            float progress = Mathf.Clamp01(normalizedTime);

            if (normalizedTime >= 1f)
            {
                entity.Anim.speed = 1f; // reset after animation ends

                if (!entity.CheckAttackTarget("Player"))
                {
                    entity.CheckFacingDirectionBasedOnTargetPos();
                    entity.stateMachine.SetNextState("CHASE", entity);
                }
                else
                {
                    entity.stateMachine.ForceSetNextState("ATTACK1", entity);
                }
            }
        }
    }

    private void StartAttack(Entity entity)
    {
        hasStartedAttack = true;
        entity.Anim.Play("Spear Goblin - Attack", 0, 0f);
        entity.SetVelocity(0);
        entity.Anim.speed = 1f;
    }
    
    public override void Exit(Entity entity)
    {
        entity.Anim.speed = 1f; // reset after animation ends
        PlayerController.Instance.defaultwalkspeed = 5.5f;
    }

}
