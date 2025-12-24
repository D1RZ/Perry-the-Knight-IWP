using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttackChecker : BaseAttackChecker
{
    public bool dashAttackHit = false;

    private HashSet<Enemy> enemiesHitThisDash = new HashSet<Enemy>();

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 7) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null) return;

        // Already hit this enemy during this dash
        if (enemiesHitThisDash.Contains(enemy)) return;

        enemiesHitThisDash.Add(enemy);

        if (enemy.isBlocking)
        {
            var InstantiatedParticle = Instantiate(ParticleManager.Instance.GetParticleEffect("Block"), enemy.BlockParticlePos);
            InstantiatedParticle.transform.localRotation = Quaternion.Euler(0f, 0f, 143f);
            InstantiatedParticle.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            return;
        }

        var hitParticle = Instantiate(
            ParticleManager.Instance.GetParticleEffect("Hit"),
            enemy.transform.position,
            Quaternion.identity
        );
        hitParticle.transform.localScale = new Vector3(-enemy.facingDirection, 1, 1);

        Debug.Log("DASH HIT!");

        StartCoroutine(DoHitImpact(enemy));
    }

    protected override IEnumerator DoHitImpact(Enemy enemy)
    {
        yield return base.DoHitImpact(enemy);

        enemy.ChangeSpriteColor(true);
        enemy.SetHealth(PlayerCombatController.Instance.GetAttackDamage());

        // Handle death
        if (enemy.health <= 0)
        {
            PlayerController.Instance.defaultwalkspeed = 7;
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathChunk"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("DeathChunk").transform.rotation);
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathBlood"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("DeathBlood").transform.rotation);
            enemy.gameObject.SetActive(false);
            hasHit = false; // reset before exiting
            yield break; // exit coroutine — nothing else to do
        }

        // Optional: freeze time
        Time.timeScale = 0f;

        // Wait for real-time duration (unaffected by timeScale)
        yield return new WaitForSecondsRealtime(0.2f);

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
            enemy.rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        if (enemy != null) enemy.ChangeSpriteColor(false);
        if (enemy != null && enemy.spriteRenderer != null)
        {
            Animator anim = enemy.spriteRenderer.GetComponent<Animator>();
            if (anim != null) anim.speed = 1;
        }

        hasHit = false;
    }

    public void ResetDashHits()
    {
        enemiesHitThisDash.Clear();
    }

}
