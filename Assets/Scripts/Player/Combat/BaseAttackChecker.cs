using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackChecker : MonoBehaviour
{
    protected HashSet<Enemy> enemiesHitThisAttack = new HashSet<Enemy>();
    private Coroutine HitImpactRoutine = null;
    [SerializeField] protected bool isSpike = false;

    protected virtual void OnEnable()
    {
        // Reset every time the hitbox is enabled (new attack)
        enemiesHitThisAttack.Clear();
    }

    // Make this virtual so child classes can override it
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer != 7 && collision.gameObject.layer != 11)) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null || !enemy.canDamage) return;

        if (enemiesHitThisAttack.Contains(enemy))
            return;

        enemiesHitThisAttack.Add(enemy);

        if (enemy.isBlocking && !isSpike)
        {
            SpawnBlockVFX(enemy);
            return;
        }

        StartCoroutine(DoHitImpact(enemy));
    }
    
    protected virtual IEnumerator DoHitImpact(Enemy enemy)
    {
        if (enemy == null || !enemy.canDamage)
            yield break;

        yield return new WaitForSeconds(0.1f);

        if (PlayerController.Instance.isHit && !isSpike)
        {
            enemiesHitThisAttack.Clear();
            yield break;
        }

        SpawnHitVFX(enemy);

        Debug.Log("LIFT HIT!");

        yield return null;
    }

    protected void SpawnHitVFX(Enemy enemy)
    {
        var hitParticle = Instantiate(
            ParticleManager.Instance.GetParticleEffect("Hit"),
            enemy.transform.position,
            Quaternion.identity
        );
        hitParticle.transform.localScale =
            new Vector3(-enemy.facingDirection, 1, 1);
    }

    protected void SpawnBlockVFX(Enemy enemy)
    {
        var blockParticle = Instantiate(
            ParticleManager.Instance.GetParticleEffect("Block"),
            enemy.BlockParticlePos
        );
        blockParticle.transform.localRotation =
            Quaternion.Euler(0f, 0f, 143f);
        blockParticle.transform.localScale =
            new Vector3(0.5f, 0.5f, 0.5f);
        enemiesHitThisAttack.Clear();
    }

}
