using UnityEngine;

public class DemonKing_SlashState : State
{
    private int remainingSlashes;
    private bool waitingForDelay = false;
    private float delayTimer;

    private float minJumpDelay = 0.15f;
    private float maxJumpDelay = 0.35f;
    private float slash_range = 8f;

    public override void Enter(Entity entity)
    {
        entity.SetVelocity(0);
        entity.Anim.speed = 1f;
        entity.hasFinishedAttack = false;
        remainingSlashes = Random.Range(3, 5);

        waitingForDelay = false;
        delayTimer = 0f;

        StartSlash(entity);
    }

    public override void onUpdate(Entity entity)
    {
        entity.rb.constraints = RigidbodyConstraints2D.None;
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Abort slash combo if player is out of range
        float distanceToPlayer = Mathf.Abs(
            PlayerController.Instance.transform.position.x - entity.transform.position.x
        );

        if (!waitingForDelay && distanceToPlayer > slash_range)
        {
            entity.hasFinishedAttack = false;
            entity.stateMachine.SetNextState("JUMP", entity);
            return;
        }

        // Waiting between last slash and jump
        if (waitingForDelay)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f)
            {
                entity.stateMachine.SetNextState("JUMP", entity);
            }
            return;
        }

        // Slash animation finished
        if (entity.hasFinishedAttack)
        {
            entity.hasFinishedAttack = false;
            remainingSlashes--;

            if (remainingSlashes > 0)
            {
                // Chain another slash
                StartSlash(entity);
            }
            else
            {
                // Finished all slashes then delay before jump
                waitingForDelay = true;
                delayTimer = Random.Range(minJumpDelay, maxJumpDelay);
            }
        }
    }

    private void StartSlash(Entity entity)
    {
        Debug.Log("SLASH AGAIN!");
        entity.Anim.speed = 1f;
        entity.CheckFacingDirectionBasedOnTargetPos();
        entity.Anim.Play("Demon_Slash", 0, 0f);
    }

    public override void Exit(Entity entity)
    {
        entity.hasFinishedAttack = false;
    }
}