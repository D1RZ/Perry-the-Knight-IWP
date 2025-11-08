using UnityEngine;

public class PauseAnimationProjectile : PauseAnimation
{
    [SerializeField] private string projectileName;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private float projectileSpeed;

    public void SpawnProjectile()
    {
        var projectile = ObjectPoolManager.Instance.Spawn(projectileName,spawnPos.position,Quaternion.identity);
        projectile.transform.localScale = new Vector3(1 * transform.parent.GetComponent<Enemy>().facingDirection, 1,1);
        projectile.GetComponent<Arrow>().SetFacingDirection(transform.parent.GetComponent<Enemy>().facingDirection);
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.parent.GetComponent<Enemy>().facingDirection * projectileSpeed, 0);
    }

}
