using System.Collections;
using UnityEngine;

public class LiftAttackChecker : BaseAttackChecker
{
    protected override IEnumerator DoHitImpact(Enemy enemy)
    {
        yield return base.DoHitImpact(enemy);

        enemy.ChangeSpriteColor(true);
        enemy.SetHealth(PlayerCombatController.Instance.GetAttackDamage());

        // Handle death
        if (enemy.health <= 0)
        {
            if (enemy.isBuilding)
            {
                PlayerController.Instance.defaultwalkspeed = 7;
                Instantiate(ParticleManager.Instance.GetParticleEffect("BuildingChunk"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("BuildingChunk").transform.rotation);
                enemy.DeadEvent();
                enemy.gameObject.SetActive(false);
                enemiesHitThisAttack.Clear();
                yield break;
            }

            PlayerController.Instance.defaultwalkspeed = 7;
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathChunk"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("DeathChunk").transform.rotation);
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathBlood"), enemy.transform.position, ParticleManager.Instance.GetParticleEffect("DeathBlood").transform.rotation);
            enemy.DeadEvent();
            enemy.gameObject.SetActive(false);
            enemiesHitThisAttack.Clear();
            yield break; // exit coroutine — nothing else to do
        }

        // Optional: freeze time
        Time.timeScale = 0f;

        // Wait for real-time duration (unaffected by timeScale)
        yield return new WaitForSecondsRealtime(0.2f);

        // Resume time
        Time.timeScale = 1f;

        // If Rigidbody was destroyed or enemy null, exit safely
        if (!enemy.isBuilding)
        {
            if (enemy == null || enemy.rb == null)
            {
                enemiesHitThisAttack.Clear();
                yield break;
            }
        }

        if (enemy != null) enemy.ChangeSpriteColor(false);

        if (enemy.canKnockUp)
        {
            enemy.rb.constraints = RigidbodyConstraints2D.None;
            enemy.rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            enemy.rb.AddForce(new Vector2(PlayerCombatController.Instance.GetKnockbackForce().x, PlayerCombatController.Instance.GetKnockbackForce().y), ForceMode2D.Impulse);
            enemy.Anim.Play(enemy.hitAnim);
            enemy.knockedUp = true;
            enemy.knockedUpGraceTimer = 0.2f;    // delay before checking grounded
            enemy.rb.gravityScale = 1.25f;
            Debug.Log("Enemy RB Velocity: " + enemy.rb.velocity);

            if (enemy != null && enemy.spriteRenderer != null)
            {
                Animator anim = enemy.spriteRenderer.GetComponent<Animator>();
                if (anim != null) anim.speed = 0;
            }
        }

        Debug.Log("LIFTED ENEMY! " + enemy.transform.rotation.z);

        yield return new WaitForSeconds(1);

        enemiesHitThisAttack.Clear();
    }

}
