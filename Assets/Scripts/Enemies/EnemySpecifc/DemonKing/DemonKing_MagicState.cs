using UnityEngine;

public class DemonKing_MagicState : State
{
    private bool isChanneling = true;
    private float spawnTimer;
    private float spawnInterval = 0.6f;  // time between spikes
    private int spikeSpawnCount = 0;
    private int forceOnPlayerEvery = 3; // every 3rd spike
    private float lastSpikeX = float.MinValue;

    // Arena bounds
    private const float MIN_X = 109.2f;
    private const float MAX_X = 136.8f;
    private const float SPIKE_Y = -81.5f;

    // Player-relative offsets
    private float minOffset = 1.2f;
    private float maxOffset = 4f;

    private float exitDelay;

    public override void Enter(Entity entity)
    {
        entity.SetVelocity(0);
        entity.rb.velocity = Vector2.zero;
        spawnTimer = 0f;
        isChanneling = true;
        exitDelay = 0.5f;
        entity.SetTarget(PlayerController.Instance.gameObject);
        entity.CheckFacingDirectionBasedOnTargetPos();
        entity.Anim.Play("Demon_Spell", 0, 0f);
        isChanneling = true;
    }

    public override void onUpdate(Entity entity)
    {
        if (!isChanneling)
        {
            exitDelay -= Time.deltaTime;

            if (exitDelay <= 0f)
            {
                entity.stateMachine.SetNextState("CHASE", entity);
            }

            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnSpike(entity);
            spawnTimer = spawnInterval;
        }

        float distanceToPlayer = Mathf.Abs(
        PlayerController.Instance.transform.position.x - entity.transform.position.x
        );

        if(distanceToPlayer <= 3.0f)
        {
            isChanneling = false;
        }
    }

    public override void Exit(Entity entity)
    {

    }

    private void SpawnSpike(Entity entity)
    {
        Transform player = PlayerController.Instance.transform;
        int facing = PlayerController.Instance.facingDirection;

        float baseX = player.position.x;

        float spawnX;

        spikeSpawnCount++;

        // Every Nth spike spawns directly on player
        if (spikeSpawnCount % forceOnPlayerEvery == 0)
        {
            spawnX = player.position.x;
        }
        else
        {
            float offset = Random.Range(minOffset, maxOffset);
            spawnX = player.position.x + (offset * facing);
        }

        spawnX = Mathf.Clamp(spawnX, MIN_X, MAX_X);

        // Prevent same spot twice (only for non-direct spikes)
        if (spikeSpawnCount % forceOnPlayerEvery != 0 &&
            Mathf.Abs(spawnX - lastSpikeX) < 0.6f)
        {
            spawnX += facing * 1.2f;
            spawnX = Mathf.Clamp(spawnX, MIN_X, MAX_X);
        }

        lastSpikeX = spawnX;

        Vector2 spawnPos = new Vector2(spawnX, SPIKE_Y);
        Object.Instantiate(ParticleManager.Instance.GetParticleEffect("Spike"), spawnPos, Quaternion.identity);
    }

}