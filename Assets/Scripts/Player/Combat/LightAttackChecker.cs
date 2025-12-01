using System.Collections;
using UnityEngine;

public class LightAttackChecker : BaseAttackChecker 
{
    [SerializeField] private bool freezeGravity = false;

    protected override IEnumerator DoHitImpact(Enemy enemy)
    {
        yield return base.DoHitImpact(enemy);

        enemy.ChangeSpriteColor(true);
        enemy.SetHealth(PlayerCombatController.Instance.GetAttackDamage());
        if (freezeGravity) enemy.rb.gravityScale = 0;

        // Handle death
        if (enemy.health <= 0)
        {
            PlayerController.Instance.defaultwalkspeed = 7;
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathChunk"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("DeathChunk").transform.rotation);
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathBlood"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("DeathBlood").transform.rotation);
            Destroy(enemy.gameObject);
            hasHit = false; // reset before exiting
            yield break; // exit coroutine — nothing else to do
        }

        // Optional: freeze time
        Time.timeScale = 0f;

        // Wait for real-time duration (unaffected by timeScale)
        yield return new WaitForSecondsRealtime(0.25f);

        // Resume time
        Time.timeScale = 1f;

        // If Rigidbody was destroyed or enemy null, exit safely
        if (enemy == null || enemy.rb == null)
        {
            hasHit = false;
            yield break;
        }

        // Apply hit impact force
        enemy.rb.constraints = RigidbodyConstraints2D.None;
        enemy.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        enemy.rb.AddForce(new Vector2(PlayerController.Instance.facingDirection * PlayerCombatController.Instance.GetKnockbackForce().x, PlayerCombatController.Instance.GetKnockbackForce().y), ForceMode2D.Impulse);
        
        // Pause enemy animation
        if (enemy.spriteRenderer != null)
        {
            Animator anim = enemy.spriteRenderer.GetComponent<Animator>();
            if (anim != null) anim.speed = 0;
        }
        
        // Wait until the enemy nearly stops sliding
        yield return new WaitForSeconds(0.1f);
        
        // Reapply constraints
        if (enemy != null && enemy.rb != null)
        {
            enemy.rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
        
        if (enemy != null) enemy.ChangeSpriteColor(false);
        if (enemy != null && enemy.spriteRenderer != null)
        {
            Animator anim = enemy.spriteRenderer.GetComponent<Animator>();
            if (anim != null) anim.speed = 1;
        }
        
        hasHit = false;

        if (freezeGravity)
        {
            yield return new WaitForSeconds(0.1f);
            enemy.rb.gravityScale = 1;
        }
    }   
}
