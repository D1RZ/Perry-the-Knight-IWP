using UnityEngine;

public class DemonKing_JumpState : State
{
    private float groundCheckTimer = 0;
    private bool hasLanded;

    public override void Enter(Entity entity)
    {
        groundCheckTimer = 0;
        hasLanded = false;
        entity.SetTarget(PlayerController.Instance.gameObject);
        entity.CheckFacingDirectionBasedOnTargetPos();
        entity.Anim.Play("Demon_Jump");
        JumpTowardPlayer(entity);
    }

    public override void onUpdate(Entity entity)
    {
        entity.Anim.SetFloat("yVelocity", entity.rb.velocity.y);

        if (hasLanded)
            return;

        if (entity.rb.velocity.y < 0)
        {
            entity.rb.gravityScale = 1.5f; // faster fall
        }
        else
        {
            entity.rb.gravityScale = 1f;   // normal rise
        }

        groundCheckTimer += Time.deltaTime;

        if (groundCheckTimer > 0.1f &&
            entity.CheckGrounded() &&
            entity.rb.velocity.y <= 0f)
        {
            hasLanded = true;

            entity.rb.velocity = Vector2.zero;
            entity.Anim.Play("Demon_Grounded");
        }
    }

    public override void Exit(Entity entity)
    {
        
    }
    
    private void JumpTowardPlayer(Entity entity)
    {
        float minJumpHeight = 2f;
        float maxJumpHeight = 5f;
        float minDistance = 0f;
        float maxDistance = 15.5f;

        float distanceToPlayer = Mathf.Abs(
            PlayerController.Instance.transform.position.x - entity.transform.position.x
        );

        float t = Mathf.InverseLerp(minDistance, maxDistance, distanceToPlayer);

        float jumpHeight = Mathf.Lerp(minJumpHeight, maxJumpHeight, t);

        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float verticalVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);

        float direction = Mathf.Sign(
            PlayerController.Instance.transform.position.x - entity.transform.position.x
        );

        float rawHorizontalDistance = Mathf.Lerp(0f, 15.5f, t);
        float horizontalDistance = (rawHorizontalDistance) * direction;

        Debug.Log("HORIZONTAL DISTANCE: " + rawHorizontalDistance);

        float timeInAir = (2f * verticalVelocity) / gravity;
        float horizontalVelocity = horizontalDistance / timeInAir;

        entity.rb.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }

}