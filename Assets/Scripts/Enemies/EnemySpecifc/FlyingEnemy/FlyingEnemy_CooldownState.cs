using UnityEngine;

public class FlyingEnemy_CooldownState : State
{
    private Vector2 startPos;
    private Vector2 controlPoint;
    private Vector2 endPos;

    private float t; // interpolation value 0 to  1
    private float duration = 1.2f; // how long the arc lasts

    private Transform player;

    private float cooldownTimer;     // hover duration after curve finishes
    private float safeDistance = 2f;        // distance enemy wants to keep
    private float horizontalRetreatSpeed = 3f; // speed to slide away during cooldown
    private bool curveFinished = false;
    private float lastRetreatDir = 1f;

    public override void Enter(Entity entity)
    {
        entity.Anim.Play("Flying Enemy - Idle");
        entity.SetVelocity(0);

        player = entity.GetTarget().transform;

        // Store the starting position
        startPos = entity.transform.position;

        // Direction away from the player
        Vector2 away = ((Vector2)entity.transform.position - (Vector2)player.position).normalized;

        float verticalDiff = entity.transform.position.y - player.position.y;

        // If enemy is high above the player then retreat downward instead of upward
        Vector2 verticalOffset;

        if (verticalDiff > 2.5f)
        {
            verticalOffset = Vector2.down * 4.5f;  // move downward
        }
        else
        {
            verticalOffset = Vector2.up * 4.5f;    // move upward
        }

        endPos = (Vector2)player.position + away * 3f + verticalOffset;

        float horizontalOvershoot = 4f; // tweak this value to change curve bends

        controlPoint = new Vector2(
            startPos.x + horizontalOvershoot * entity.facingDirection,
            startPos.y
        );

        float diffX = player.position.x - entity.transform.position.x;
        if (Mathf.Abs(diffX) > 0.1f) // avoid jitter when directly above
        {
            int desiredDir = diffX > 0 ? 1 : -1;

            if (entity.facingDirection != desiredDir)
            {
                entity.facingDirection = desiredDir;

                Vector3 scale = entity.transform.localScale;
                scale.x = Mathf.Abs(scale.x) * desiredDir;
                entity.transform.localScale = scale;
            }
        }

        curveFinished = false;
        cooldownTimer = Random.Range(0.4f, 0.8f);

        // Reset curve progress
        t = 0f;
    }

    public override void onUpdate(Entity entity)
    {
        if (PlayerController.Instance._PlayerData.HealthData <= 0) return;

        if (!curveFinished)
        {
            // Progress along curve
            t += Time.deltaTime / duration;
            t = Mathf.Clamp01(t);

            // Quadratic Bezier formula
            Vector2 curvePos =
                Mathf.Pow(1 - t, 2) * startPos +
                2 * (1 - t) * t * controlPoint +
                Mathf.Pow(t, 2) * endPos;

            // Move entity
            entity.transform.position = curvePos;

            // When the curve finishes, return to chase
            if (t >= 1f)
            {
                curveFinished = true;
            }
            return;
        }

        cooldownTimer -= Time.deltaTime;

        // Check player distance
        float dist = Vector2.Distance(entity.transform.position, player.position);

        if (dist < safeDistance)
        {
            // Horizontal difference
            float diffX = entity.transform.position.x - player.position.x;

            // Only flip if enemy is not directly above player
            float dirX;

            // Dead-zone fix to prevent jitter
            if (Mathf.Abs(diffX) < 0.2f)
            {
                dirX = lastRetreatDir;
            }
            else
            {
                dirX = Mathf.Sign(diffX);
                lastRetreatDir = dirX;
            }

            entity.transform.position += new Vector3(
                dirX * horizontalRetreatSpeed * Time.deltaTime,
                0,
                0
            );
        }

        if (cooldownTimer <= 0f)
        {
            entity.stateMachine.SetNextState("CHASE", entity);
        }
    }

    public override void Exit(Entity entity)
    {
        // Stop motion on exit
        entity.rb.velocity = Vector2.zero;
    }

}
