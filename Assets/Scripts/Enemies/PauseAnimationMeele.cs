using System.Collections;
using UnityEngine;

public class PauseAnimationMeele : PauseAnimation
{
    private int latestAttack;
    private bool hasHit = false;
    [SerializeField] private bool isFlying = false;
    private bool FlyHit = false; // for flying enemy as flying enemy attack hitbox is more than one frame
    private bool Parried = false; // checks for if player has parried enemy attack
    private bool Blocked = false;
    [SerializeField] private Entity entity;

    public void AttackingVFX(int VFXAttackNo = 1) // which vfx to instantiate
    {
        latestAttack = VFXAttackNo;
        transform.parent.GetComponent<Enemy>().AttackVFX(VFXAttackNo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit ||PlayerController.Instance.GetIsRolling() || collision.gameObject.layer != 8 || PlayerController.Instance.GetIsDashAttacking()) return;

        if (isFlying && !FlyHit) FlyHit = true;
        else if (isFlying && FlyHit) return;

        hasHit = true;
   
        if (CheckForBlockOrParry()) return;

        Debug.Log("HIT REGISTERED!!");
        var InstantiatedParticle = Instantiate(ParticleManager.Instance.GetParticleEffect("Hit"), PlayerController.Instance.gameObject.transform.position,Quaternion.identity);
        InstantiatedParticle.transform.localScale = new Vector3(-PlayerController.Instance.facingDirection,1,1);
        Enemy enemy = entity as Enemy;
        if (enemy.health > 0)
        {
            StartCoroutine(DoHitImpact());
        }
    }

    private bool CheckForBlockOrParry()
    {
        if(PlayerController.Instance.GetIsBlocking() && transform.parent.GetComponent<Enemy>().facingDirection != PlayerController.Instance.facingDirection)
        {
            if (Time.time - PlayerController.Instance.GetStartBlockTime() <= 0.3f)
            {
                Enemy enemy = transform.parent.GetComponent<Enemy>();
                var InstantiatedParticle = Instantiate(ParticleManager.Instance.GetParticleEffect("Hit"), enemy.gameObject.transform.position, Quaternion.identity);
                InstantiatedParticle.transform.localScale = new Vector3(-enemy.facingDirection, 1, 1);
                Parried = true;
                PlayerController.Instance.parrySuccess = true;
                PlayerController.Instance.blockSuccess = true;
                PlayerController.Instance.animationController.animator.SetBool("Block", true);
                StartCoroutine(DoParryHitImpact(enemy));
                return true;
            }
            else
            {
                Blocked = true;
                PlayerController.Instance.parrySuccess = false;
                PlayerController.Instance.blockSuccess = true;
                PlayerController.Instance.animationController.animator.SetBool("Block", true);
                Enemy enemy = transform.parent.GetComponent<Enemy>();
                var InstantiatedParticle = Instantiate(ParticleManager.Instance.GetParticleEffect("Hit"), enemy.gameObject.transform.position, Quaternion.identity);
                InstantiatedParticle.transform.localScale = new Vector3(-enemy.facingDirection, 1, 1);
                StartCoroutine(DoParryHitImpact(enemy));
                return false;
            }
        }

        return false;
    }

    private IEnumerator DoHitImpact()
    {
        transform.GetComponent<Collider2D>().enabled = false;
        if(!Blocked) PlayerController.Instance.ChangeSpriteColor(true);
        PlayerController.Instance.OnHit();
        transform.parent.GetComponent<Enemy>().HitConnected(latestAttack);
        PlayerController.InvokeOnPlayerHit();

        if (PlayerController.Instance._PlayerData.HealthData <= 0)
        {
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathChunk"), PlayerController.Instance.transform.position, ParticleManager.Instance.GetParticleEffect("DeathChunk").transform.rotation);
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathBlood"), PlayerController.Instance.transform.position, ParticleManager.Instance.GetParticleEffect("DeathBlood").transform.rotation);
            transform.parent.GetComponent<Enemy>().stateMachine.SetNextState("IDLE", transform.parent.GetComponent<Enemy>());
            yield return null;
        }

        if (!Blocked)
        {
            // Optional: freeze time
            Time.timeScale = 0f;

            // Shake camera (we’ll handle timing using real time)
            //CameraShake.Instance.Shake(0.2f, 0.5f); // duration, intensity

            // Wait for real-time duration (unaffected by timeScale)
            yield return new WaitForSecondsRealtime(0.2f);

            // Resume time
            Time.timeScale = 1f;
        }

        PlayerController.Instance.SetCanMove(false); // or a HitStun flag
        PlayerController.Instance.rb.constraints = RigidbodyConstraints2D.None;
        PlayerController.Instance.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        PlayerController.Instance.rb.velocity = Vector3.zero;
        PlayerController.Instance.rb.AddForce(new Vector2(transform.parent.GetComponent<Enemy>().facingDirection * 5f, 0), ForceMode2D.Impulse);
        PlayerController.Instance.animationController.animator.speed = 0;
        // Wait until the player nearly stops sliding
        Rigidbody2D rb = PlayerController.Instance.rb;
        yield return new WaitForSeconds(0.1f);
        PlayerController.Instance.SetCanMove(true);
        if (!Blocked) PlayerController.Instance.ChangeSpriteColor(false);
        Blocked = false;
        PlayerController.Instance.SetIsHit(false);
        PlayerController.Instance.animationController.animator.speed = 1;
        hasHit = false;
    }

    private IEnumerator DoParryHitImpact(Enemy enemy)
    {
        Collider2D hitboxCollider = transform.GetComponent<Collider2D>();
        if (hitboxCollider != null) hitboxCollider.enabled = false;

        // Early exit if enemy already null
        if (enemy == null)
        {
            hasHit = false;
            yield break;
        }

        enemy.ChangeSpriteColor(true);
        enemy.SetHealth(10); // parry damage

        // Handle death
        if (enemy.health <= 0)
        {
            PlayerController.Instance.defaultwalkspeed = 7;
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathChunk"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("DeathChunk").transform.rotation);
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathBlood"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("DeathBlood").transform.rotation);
            enemy.gameObject.SetActive(false);
            hasHit = false; // reset before exiting
            PlayerController.Instance.RecoverFromBlockOrParry();
            yield break; // exit coroutine — nothing else to do
        }

        if (Parried)
        {
            // Optional: freeze time
            Time.timeScale = 0f;

            // Wait for real-time duration (unaffected by timeScale)
            yield return new WaitForSecondsRealtime(0.2f);

            // Resume time
            Time.timeScale = 1f;
        }

        // If Rigidbody was destroyed or enemy null, exit safely
        if (enemy == null || enemy.rb == null)
        {
            hasHit = false;
            yield break;
        }

        // Apply hit impact force
        enemy.rb.constraints = RigidbodyConstraints2D.None;
        enemy.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        enemy.rb.AddForce(new Vector2(PlayerController.Instance.facingDirection * 10f, 0), ForceMode2D.Impulse);

        // Pause enemy animation
        if (enemy.spriteRenderer != null)
        {
            Animator anim = enemy.spriteRenderer.GetComponent<Animator>();
            if (anim != null) anim.speed = 0;
        }

        // Wait until the enemy nearly stops sliding
        yield return new WaitForSeconds(0.15f);

        // Reapply constraints
        if (enemy != null && enemy.rb != null)
        {
            enemy.rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        if (enemy != null) enemy.ChangeSpriteColor(false);
        if (enemy != null && enemy.spriteRenderer != null)
        {
            Animator anim = enemy.spriteRenderer.GetComponent<Animator>();
            if (anim != null) anim.speed = 1;
        }

        if (Parried)
        {
            Parried = false;
            enemy.Anim.Play(enemy.hitAnim);
            enemy.stunTimer = enemy.stunTime;
            enemy.isStunned = true;
            enemy.rb.velocity = Vector3.zero;
            enemy.isBlocking = false;
        }

        hasHit = false;
    }

    public void SetFlyHit(bool flyhit)
    {
        FlyHit = flyhit;
    }

    public void SetFinishedAttack() // for skeleton enemy
    {
        entity.hasFinishedAttack = true;
    }

}
