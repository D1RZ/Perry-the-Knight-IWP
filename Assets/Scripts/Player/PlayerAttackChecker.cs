using System.Collections;
using UnityEngine;

public class PlayerAttackChecker : MonoBehaviour
{
    private float damage;
    private bool hasHit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TRIGGER ENTERED!" + collision.gameObject.name);
        if (hasHit || collision.gameObject.layer != 7) return;
        hasHit = true;
        Enemy enemy = collision.GetComponent<Enemy>();
        var InstantiatedParticle = Instantiate(ParticleManager.Instance.GetParticleEffect("Hit"),enemy.gameObject.transform.position,Quaternion.identity);
        InstantiatedParticle.transform.localScale = new Vector3(-enemy.facingDirection, 1, 1);
        StartCoroutine(DoHitImpact(enemy));
    }
    
    private IEnumerator DoHitImpact(Enemy enemy)
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
        enemy.SetHealth(damage);

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
        enemy.rb.AddForce(new Vector2(PlayerController.Instance.facingDirection * 15f, 0), ForceMode2D.Impulse);

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
    }

    public void SetDamage(float Damage)
    {
        damage = Damage;
    }

    public float GetDamage()
    {
        return damage;
    }

}
