using UnityEngine;

public class DemonSlime_ChaseState : State
{
    private ChaseState_Data slimeChaseData;
    private bool resetChase = false; // after getting stunned
    private float chaseTimer;
    private float dashTransitionTime = 1.5f;
    private float magicTimer = 0f;
    private float magicCooldown = 5.5f;

    public DemonSlime_ChaseState(ChaseState_Data stateData)
    {
        this.slimeChaseData = stateData;
    }

    public override void Enter(Entity entity)
    {
        if (!entity.CheckGrounded())
        {
            resetChase = true;
            return;
        }
        chaseTimer = 0;
        dashTransitionTime = 2;
        resetChase = false;
        entity.transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
        entity.Anim.Play("Slime_Move");
        entity.SetTarget(PlayerController.Instance.gameObject);
        entity.isChasing = true;
        float direction = Mathf.Sign(entity.GetTarget().transform.position.x - entity.transform.position.x);
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        entity.SetVelocity(direction * slimeChaseData.ChaseMovementSpeed);
        entity.CheckFacingDirection();
    }

    public override void onUpdate(Entity entity)
    {
        if (entity.CheckGrounded() && resetChase)
        {
            resetChase = false;
            chaseTimer = 0;
            dashTransitionTime = 1;
            entity.transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
            entity.Anim.Play("Slime_Move");
            entity.SetTarget(PlayerController.Instance.gameObject);
            entity.isChasing = true;
            float direction = Mathf.Sign(entity.GetTarget().transform.position.x - entity.transform.position.x);
            entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            entity.SetVelocity(direction * slimeChaseData.ChaseMovementSpeed);
            entity.CheckFacingDirection();
            return;
        }
        if(entity.GetTarget() != null) entity.CheckFacingDirectionBasedOnTargetPos();
        entity.SetVelocity(entity.facingDirection * slimeChaseData.ChaseMovementSpeed);

        chaseTimer += Time.deltaTime;
        magicTimer += Time.deltaTime;

        if (magicTimer >= magicCooldown)
        {
            magicTimer = 0f;
            chaseTimer = 0f;

            entity.stateMachine.SetNextState("MAGICATTACK", entity);
            return;
        }

        if (chaseTimer >= dashTransitionTime)
        {
            chaseTimer = 0;
            entity.stateMachine.SetNextState("DASHATTACK", entity);
            return;
        }

    }

    public override void Exit(Entity entity)
    {
        entity.isChasing = false;
    }

}
