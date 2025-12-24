    using UnityEngine;

    public class FlyingEnemy_IdleState : IdleState
    {
        public FlyingEnemyIdleState_Data flyingEnemyIdleData;

        // Hover logic variables
        private float hoverTimer;
        private float startY;
        // Randomised amplitude unique for each enemy
        private float hoverAmplitude;
        
        public FlyingEnemy_IdleState(FlyingEnemyIdleState_Data stateData)
        {
            flyingEnemyIdleData = stateData;
        }
        
        public override void Enter(Entity entity)
        {
            entity.Anim.Play("Flying Enemy - Idle");
            entity.SetVelocity(0);
            entity.spriteRenderer.GetComponent<TrailRenderer>().enabled = false;
            
            // Reset the hover timer
            hoverTimer = 0f;
            
            // Store starting height
            startY = entity.transform.position.y;

            // Randomize amplitude once on enter
            hoverAmplitude = Random.Range(
                flyingEnemyIdleData.minHoverAmplitude,
                flyingEnemyIdleData.maxHoverAmplitude
            );
        }

        public override void onUpdate(Entity entity)
        {
            hoverTimer += Time.deltaTime * flyingEnemyIdleData.hoverSpeed;

            float offset = Mathf.Sin(hoverTimer) * hoverAmplitude;

            Vector3 pos = entity.transform.position;
            pos.y = startY + offset;
            entity.transform.position = pos;

            // Check for transitions
            if (PlayerController.Instance._PlayerData.HealthData > 0)
            {
                if (entity.CheckChaseOverlapTarget("Player"))
                {
                    if (entity.CheckAttackTarget("Player"))
                    {
                        entity.stateMachine.SetNextState("ATTACK", entity);
                        return;
                    }

                    entity.stateMachine.SetNextState("CHASE", entity);
                    return;
                }
            }
        }

        public override void Exit(Entity entity)
        {
            // No special exit logic needed
        }

    }