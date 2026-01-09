using UnityEngine;

public class DemonSlime_MagicAttackState : State
{
    private Vector2 startPos;
    private Vector2 targetPos;

    private float liftHeight = 5f;
    private float liftDuration = 0.4f;
    private float liftTimer;

    private bool hasLifted = false;
    private GameObject spike;

    private bool isChanneling = false;

    private float channelTimer;
    private float spawnTimer;

    private float channelDuration = 10f;     // total magic time
    private float spawnInterval = 0.8f;       // time between spikes
    private int spikeSpawnCount = 0;
    private int forceOnPlayerEvery = 3; // every 3rd spike

    private float lastSpikeX = float.MinValue;

    // Arena bounds
    private const float MIN_X = 109.2f;
    private const float MAX_X = 136.8f;
    private const float SPIKE_Y = -81.5f;

    // Player-relative offsets
    private float minOffset = 1.2f;
    private float maxOffset = 5f;

    public DemonSlime_MagicAttackState(GameObject spike)
    {
        this.spike = spike;
    }

    public override void Enter(Entity entity)
    {
        entity.SetVelocity(0);
        entity.rb.velocity = Vector2.zero;
        entity.rb.gravityScale = 0f;
        entity.transform.GetComponent<Collider2D>().enabled = false;

        startPos = entity.transform.position;
        targetPos = startPos + Vector2.up * liftHeight;

        liftTimer = 0f;
        hasLifted = false;

        isChanneling = false;
        channelTimer = channelDuration;
        spawnTimer = 0f;

        entity.Anim.Play("Slime_Dash");
    }

    public override void onUpdate(Entity entity)
    {
        if (!hasLifted)
        {
            liftTimer += Time.deltaTime;
            float t = liftTimer / liftDuration;

            entity.transform.position = Vector2.Lerp(
                startPos,
                targetPos,
                Mathf.SmoothStep(0, 1, t)
            );

            if (t >= 1f)
            {
                hasLifted = true;
                entity.transform.GetChild(2).gameObject.SetActive(true);
                StartMagic(entity);
            }
            return;
        }

        if(isChanneling)
        {
            channelTimer -= Time.deltaTime;
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f)
            {
                SpawnSpike(entity);
                spawnTimer = spawnInterval;
            }

            if (channelTimer <= 0f)
            {
                isChanneling = false;
                entity.transform.GetChild(2).gameObject.SetActive(false);
                entity.stateMachine.SetNextState("CHASE", entity);
            }
        }

    }

    private void StartMagic(Entity entity)
    {
        isChanneling = true;
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
        Object.Instantiate(spike, spawnPos, Quaternion.identity);
    }

    public override void Exit(Entity entity)
    {
        entity.transform.GetComponent<Collider2D>().enabled = true;
        entity.rb.gravityScale = 1f;
    }
    
}