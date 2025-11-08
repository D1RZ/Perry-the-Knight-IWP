using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float Damage;
    private bool hasHit = false;
    private float damageMultiplier;
    private int facingDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 4)
        {
            ReturnToPool();
            return;
        }

        if (hasHit || PlayerController.Instance.GetIsRolling() || collision.gameObject.layer != 8) return;

       damageMultiplier = 1;

       hasHit = true;

       if (CheckForBlockOrParry()) return;

       Debug.Log("HIT REGISTERED!!");
       var InstantiatedParticle = Instantiate(ParticleManager.Instance.GetParticleEffect("Hit"), PlayerController.Instance.gameObject.transform.position, Quaternion.identity);
       InstantiatedParticle.transform.localScale = new Vector3(-PlayerController.Instance.facingDirection, 1, 1);
       StartCoroutine(DoHitImpact(damageMultiplier));
    }

    private bool CheckForBlockOrParry()
    {
        if (PlayerController.Instance.GetIsBlocking())
        {
            PlayerController.Instance.blockSuccess = true;
            PlayerController.Instance.animationController.animator.SetBool("Block", true);

            if (Time.time - PlayerController.Instance.GetStartBlockTime() <= 0.4f)
            {
                transform.GetComponent<Animator>().SetTrigger("Block");
                var rb = transform.GetComponent<Rigidbody2D>();
                rb.velocity = new Vector2(-facingDirection,-1);
                return true;
            }
            else
            {
                damageMultiplier = 0.2f;
                return false;
            }

        }

        return false;
    }

    private IEnumerator DoHitImpact(float multiplier)
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetComponent<Collider2D>().enabled = false;
        PlayerController.Instance.ChangeSpriteColor(true);
        PlayerController.Instance.OnHit();
        PlayerController.Instance._PlayerData.HealthData -= (multiplier * Damage);
        PlayerController.InvokeOnPlayerHit();

        if (PlayerController.Instance._PlayerData.HealthData <= 0)
        {
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathChunk"), PlayerController.Instance.transform.position, ParticleManager.Instance.GetParticleEffect("DeathChunk").transform.rotation);
            Instantiate(ParticleManager.Instance.GetParticleEffect("DeathBlood"), PlayerController.Instance.transform.position, ParticleManager.Instance.GetParticleEffect("DeathBlood").transform.rotation);
            yield return null;
        }

        // Optional: freeze time
        Time.timeScale = 0f;

        // Shake camera (we’ll handle timing using real time)
        //CameraShake.Instance.Shake(0.2f, 0.5f); // duration, intensity

        // Wait for real-time duration (unaffected by timeScale)
        yield return new WaitForSecondsRealtime(0.2f);

        // Resume time
        Time.timeScale = 1f;
        PlayerController.Instance.SetCanMove(false); // or a HitStun flag
        PlayerController.Instance.rb.constraints = RigidbodyConstraints2D.None;
        PlayerController.Instance.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        PlayerController.Instance.rb.velocity = Vector3.zero;
        PlayerController.Instance.rb.AddForce(new Vector2(facingDirection * 15f, 0), ForceMode2D.Impulse);
        PlayerController.Instance.animationController.animator.speed = 0;
        // Wait until the player nearly stops sliding
        Rigidbody2D rb = PlayerController.Instance.rb;
        yield return new WaitForSeconds(0.1f);
        PlayerController.Instance.SetCanMove(true);
        PlayerController.Instance.ChangeSpriteColor(false);
        PlayerController.Instance.SetIsHit(false);
        PlayerController.Instance.animationController.animator.speed = 1;
        ObjectPoolManager.Instance.ReturnToPool(this.gameObject);
    }

    public void SetFacingDirection(int direction)
    {
        facingDirection = direction;
    }

    public void ReturnToPool() // when arrow is parried
    {
        ObjectPoolManager.Instance.ReturnToPool(this.gameObject);
    }

}
