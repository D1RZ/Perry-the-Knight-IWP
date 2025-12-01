using System.Collections;
using UnityEngine;

public class BaseAttackChecker : MonoBehaviour
{
    protected bool hasHit = false;

    // Make this virtual so child classes can override it
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit || collision.gameObject.layer != 7) return;

        hasHit = true;

        Collider2D hitboxCollider = transform.GetComponent<Collider2D>();
        if (hitboxCollider != null) hitboxCollider.enabled = false;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null) return;

        var hitParticle = Instantiate(
            ParticleManager.Instance.GetParticleEffect("Hit"),
            enemy.transform.position,
            Quaternion.identity
        );
        hitParticle.transform.localScale = new Vector3(-enemy.facingDirection, 1, 1);

        Debug.Log("LIFT HIT!");

        StartCoroutine(DoHitImpact(enemy));
    }

    protected virtual IEnumerator DoHitImpact(Enemy enemy)
    {
        // Early exit if enemy already null
        if (enemy == null)
        {
            hasHit = false;
            yield break;
        }
    }

}
