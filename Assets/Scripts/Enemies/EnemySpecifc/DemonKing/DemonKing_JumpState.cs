using System.Collections.Generic;
using UnityEngine;

public class DemonKing_JumpState : State
{
    private float groundCheckTimer = 0;
    private bool hasLanded;
    private bool spawnedSpikes = false;
    private bool spawningSpikesOutward = true;
    private int LeftIndex = 0;
    private int RightIndex = 0;
    private float LeftSpikesLength;
    private float RightSpikesLength;
    private bool LeftSpawnDone = false;
    private bool RightSpawnDone = false;
    private bool LeftRetractDone = false;
    private bool RightRetractDone = false;
    private List<GameObject> LeftEarthSpikes = new List<GameObject>();
    private List<GameObject> RightEarthSpikes = new List<GameObject>();
    private PauseAnimationMeele CurrentLeftEarthSpike;
    private PauseAnimationMeele CurrentRightEarthSpike;
    private int remainingJumps;

    public override void Enter(Entity entity)
    {
        groundCheckTimer = 0;
        hasLanded = false;
        spawnedSpikes = false;
        spawningSpikesOutward = true;
        LeftIndex = 0;
        RightIndex = 0;
        LeftEarthSpikes.Clear();
        RightEarthSpikes.Clear();
        CurrentLeftEarthSpike = null;
        CurrentRightEarthSpike = null;
        LeftSpawnDone = false;
        RightSpawnDone = false;
        LeftRetractDone = false;
        RightRetractDone = false;
        entity.startAttack = false;
        PlayerController.Instance.attackedByTrail = false;
        remainingJumps = Random.Range(2, 4);
        entity.SetTarget(PlayerController.Instance.gameObject);
        entity.CheckFacingDirectionBasedOnTargetPos();
        entity.Anim.Play("Demon_Jump");
        JumpTowardPlayer(entity);
    }

    public override void onUpdate(Entity entity)
    {
        DemonKing demonKing = entity as DemonKing;

        entity.Anim.SetFloat("yVelocity", entity.rb.velocity.y);
        
        if(entity.startAttack && !spawnedSpikes)
        {
            int LeftSpikesAmount = Mathf.CeilToInt(LeftSpikesLength / 1.4f);
            int RightSpikesAmount = Mathf.CeilToInt(RightSpikesLength / 1.4f);

            Debug.Log("LEFT SPIKES AMT: " + LeftSpikesAmount);

            Debug.Log("RIGHT SPIKES AMT: " + RightSpikesAmount);

            for(int i = 0; i < LeftSpikesAmount; i++)
            {
                GameObject earthSpike = ObjectPoolManager.Instance.Spawn("Earth Spikes",new Vector3(demonKing.spikeLeftSpawn.transform.position.x - (1.4f * i),demonKing.spikeLeftSpawn.transform.position.y), Quaternion.identity, false);
                earthSpike.transform.SetParent(demonKing.spikeLeftSpawn.transform);
                earthSpike.transform.position = new Vector3(demonKing.spikeLeftSpawn.transform.position.x - (1.4f * i), demonKing.spikeLeftSpawn.transform.position.y, 0);
                earthSpike.GetComponent<SpriteRenderer>().sortingOrder = LeftSpikesAmount - i;
                earthSpike.transform.localScale = new Vector3(-1,1,1);
                earthSpike.GetComponent<PauseAnimationMeele>().startedAttack = false;
                earthSpike.GetComponent<PauseAnimationMeele>().finishedAttack = false;
                LeftEarthSpikes.Add(earthSpike);
            }

            for(int j = 0; j < RightSpikesAmount; j++)
            {
                GameObject earthSpike = ObjectPoolManager.Instance.Spawn("Earth Spikes", new Vector3(demonKing.spikeRightSpawn.transform.position.x + (1.4f * j), demonKing.spikeRightSpawn.transform.position.y), Quaternion.identity, false);
                earthSpike.transform.SetParent(demonKing.spikeRightSpawn.transform);
                earthSpike.transform.position = new Vector3(demonKing.spikeLeftSpawn.transform.position.x + (1.4f * j), demonKing.spikeLeftSpawn.transform.position.y, 0);
                earthSpike.GetComponent<SpriteRenderer>().sortingOrder = RightSpikesAmount - j;
                earthSpike.transform.localScale = new Vector3(1, 1, 1);
                earthSpike.GetComponent<PauseAnimationMeele>().startedAttack = false;
                earthSpike.GetComponent<PauseAnimationMeele>().finishedAttack = false;
                RightEarthSpikes.Add(earthSpike);
            }

            spawnedSpikes = true;
        }

        if (spawnedSpikes)
        {
            if (spawningSpikesOutward) UpdateSpikesOutwardSpread();
            else UpdateSpikesRetraction(entity);
        }

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
            entity.Anim.Play("Demon_Grounded", 0, 0f);
            LeftSpikesLength = 25;
            RightSpikesLength = 25;
        }
    }
    
    public override void Exit(Entity entity)
    {
        entity.startAttack = false;

        for (int i = 0; i < LeftEarthSpikes.Count; i++)
        {
            ObjectPoolManager.Instance.ReturnToPool(LeftEarthSpikes[i]);
        }

        for (int i = 0; i < RightEarthSpikes.Count; i++)
        {
            ObjectPoolManager.Instance.ReturnToPool(RightEarthSpikes[i]);
        }
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

        entity.rb.constraints = RigidbodyConstraints2D.None;
        entity.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        entity.rb.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }

    private void UpdateSpikesOutwardSpread()
    {
        if(LeftIndex <= LeftEarthSpikes.Count - 1)
        {
            if (!CurrentLeftEarthSpike)
            {
                CurrentLeftEarthSpike = LeftEarthSpikes[LeftIndex].GetComponent<PauseAnimationMeele>();
                CurrentLeftEarthSpike.gameObject.SetActive(true);
            }
            else
            {
                if (CurrentLeftEarthSpike.startedAttack)
                {
                    CurrentLeftEarthSpike.GetComponent<Animator>().speed = 0;

                    if (LeftIndex < LeftEarthSpikes.Count - 1)
                    {
                        LeftIndex++;
                        CurrentLeftEarthSpike = LeftEarthSpikes[LeftIndex].GetComponent<PauseAnimationMeele>();
                        CurrentLeftEarthSpike.gameObject.SetActive(true);
                        CurrentLeftEarthSpike.GetComponent<Animator>().speed = 1;
                        Debug.Log("NEXT LEFT!");
                    }
                    else
                    {
                        LeftSpawnDone = true;
                    }
                }
            }
        }

        if (RightIndex <= RightEarthSpikes.Count - 1)
        {
            if (!CurrentRightEarthSpike)
            {
                CurrentRightEarthSpike = RightEarthSpikes[RightIndex].GetComponent<PauseAnimationMeele>();
                CurrentRightEarthSpike.gameObject.SetActive(true);
            }
            else
            {
                if (CurrentRightEarthSpike.startedAttack)
                {
                    CurrentRightEarthSpike.GetComponent<Animator>().speed = 0;

                    if (RightIndex < RightEarthSpikes.Count - 1)
                    {
                        RightIndex++;
                        CurrentRightEarthSpike = RightEarthSpikes[RightIndex].GetComponent<PauseAnimationMeele>();
                        CurrentRightEarthSpike.gameObject.SetActive(true);
                        CurrentRightEarthSpike.GetComponent<Animator>().speed = 1;
                        Debug.Log("NEXT RIGHT!");
                    }
                    else
                    {
                        RightSpawnDone = true;
                    }
                }
            }
        }

        if(LeftSpawnDone && RightSpawnDone)
        {
            spawningSpikesOutward = false;
        }
    }

    private void UpdateSpikesRetraction(Entity entity)
    {
        if(LeftIndex >= 0)
        {
            CurrentLeftEarthSpike.GetComponent<Animator>().speed = 1;

            if(CurrentLeftEarthSpike.finishedAttack)
            {
                if (LeftIndex > 0)
                {
                    LeftIndex--;
                    CurrentLeftEarthSpike.gameObject.SetActive(false);
                    CurrentLeftEarthSpike = LeftEarthSpikes[LeftIndex].GetComponent<PauseAnimationMeele>();
                }
                else
                {
                    CurrentLeftEarthSpike.gameObject.SetActive(false);
                    LeftRetractDone = true;
                }
            }
        }

        if (RightIndex >= 0)
        {
            CurrentRightEarthSpike.GetComponent<Animator>().speed = 1;

            if (CurrentRightEarthSpike.finishedAttack)
            {
                if(RightIndex > 0)
                {
                    RightIndex--;
                    CurrentRightEarthSpike.gameObject.SetActive(false);
                    CurrentRightEarthSpike = RightEarthSpikes[RightIndex].GetComponent<PauseAnimationMeele>();
                }
                else
                {
                    CurrentRightEarthSpike.gameObject.SetActive(false);
                    RightRetractDone = true;
                }
            }
        }

        if (LeftRetractDone && RightRetractDone)
        {
            PlayerController.Instance.SetCanMove(true);
            PlayerController.Instance.ChangeSpriteColor(false);
            PlayerController.Instance.SetIsHit(false);
            PlayerController.Instance.animationController.animator.speed = 1;
            spawnedSpikes = false;
            entity.startAttack = false;

            remainingJumps--;

            if (remainingJumps > 0)
            {
                // RESET FOR NEXT JUMP
                ResetForNextJump(entity);
            }
            else
            {
                // DONE JUMPING
                entity.stateMachine.SetNextState("MAGIC", entity);
            }
        }
    }

    private void ResetForNextJump(Entity entity)
    {
        // Reset per-jump state
        hasLanded = false;
        groundCheckTimer = 0;

        spawningSpikesOutward = true;

        LeftIndex = 0;
        RightIndex = 0;

        LeftSpawnDone = false;
        RightSpawnDone = false;
        LeftRetractDone = false;
        RightRetractDone = false;

        LeftEarthSpikes.Clear();
        RightEarthSpikes.Clear();

        CurrentLeftEarthSpike = null;
        CurrentRightEarthSpike = null;

        PlayerController.Instance.attackedByTrail = false;
        entity.SetTarget(PlayerController.Instance.gameObject);
        entity.CheckFacingDirectionBasedOnTargetPos();
        entity.Anim.Play("Demon_Jump", 0, 0f);
        JumpTowardPlayer(entity);
    }

}