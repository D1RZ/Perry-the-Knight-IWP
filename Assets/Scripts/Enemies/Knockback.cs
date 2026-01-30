using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float upwardForce = 2f;
    private bool knockBack = false;
    private bool Blocked = false;
    private bool Parried = false;
    public bool isDashing = false;

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (!other.CompareTag("Player") || knockBack || isDashing || PlayerController.Instance.GetIsDashAttacking()) return;

    //    knockBack = true;

    //    if (CheckForBlockOrParry()) return;

    //    ApplyKnockback(other.transform.position,0.2f);
    //}
    
    public void ApplyKnockback(Vector2 sourcePos, float duration)
    {
        StartCoroutine(KnockbackRoutine(sourcePos, duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 sourcePos, float duration)
    {
        transform.GetComponent<Collider2D>().enabled = false;
        PlayerController.Instance.ChangeSpriteColor(true);
        PlayerController.Instance.OnHit();
        transform.parent.GetComponent<Enemy>().HitConnected(0);
        PlayerController.InvokeOnPlayerHit();

        if (PlayerController.Instance._PlayerData.HealthData <= 0)
        {
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathChunk"), PlayerController.Instance.transform.position, ParticleManager.Instance.GetParticleEffect("DeathChunk").transform.rotation);
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathBlood"), PlayerController.Instance.transform.position, ParticleManager.Instance.GetParticleEffect("DeathBlood").transform.rotation);
            yield return null;
        }

        if (!Blocked)
        {
            // Optional: freeze time
            Time.timeScale = 0f;

            // Shake camera (we’ll handle timing using real time)
            //CameraShake.Instance.Shake(0.2f, 0.5f); // duration, intensity

            // Wait for real-time duration (unaffected by timeScale)
            yield return new WaitForSecondsRealtime(0.1f);

            // Resume time
            Time.timeScale = 1f;
        }

        PlayerController.Instance.SetCanMove(false);
        PlayerController.Instance.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        PlayerController.Instance.rb.velocity = Vector2.zero;

        Vector2 direction = (sourcePos - (Vector2)transform.parent.position);

        // Horizontal priority
        float dirX = Mathf.Sign(direction.x);

        // If almost directly above, push upward instead
        float dirY = direction.y > 0.2f ? 1f : 0f;

        // Fallback: never allow zero force
        if (dirX == 0 && dirY == 0)
            dirX = PlayerController.Instance.facingDirection;

        Vector2 knockDir = new Vector2(dirX, dirY).normalized;
        PlayerController.Instance.rb.AddForce(new Vector2(knockDir.x * knockbackForce,knockDir.y * upwardForce), ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        PlayerController.Instance.SetCanMove(true);
        if(!Blocked) PlayerController.Instance.ChangeSpriteColor(false);
        Blocked = false;
        PlayerController.Instance.SetIsHit(false);
        PlayerController.Instance.animationController.animator.speed = 1;
        transform.GetComponent<Collider2D>().enabled = true;
        knockBack = false;
    }

    private bool CheckForBlockOrParry()
    {
        if (PlayerController.Instance.GetIsBlocking() && transform.parent.GetComponent<Enemy>().facingDirection != PlayerController.Instance.facingDirection)
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

    private IEnumerator DoParryHitImpact(Enemy enemy)
    {
        Collider2D hitboxCollider = transform.GetComponent<Collider2D>();
        if (hitboxCollider != null) hitboxCollider.enabled = false;

        // Early exit if enemy already null
        if (enemy == null)
        {
            knockBack = false;
            yield break;
        }

        enemy.ChangeSpriteColor(true);
        enemy.SetHealth(10); // parry damage

        // Handle death
        if (enemy.health <= 0)
        {
            PlayerController.Instance.defaultwalkspeed = 9;
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathChunk"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("DeathChunk").transform.rotation);
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathBlood"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("DeathBlood").transform.rotation);
            enemy.gameObject.SetActive(false);
            knockBack = false; // reset before exiting
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
            knockBack = false;
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

        knockBack = false;
    }

}
