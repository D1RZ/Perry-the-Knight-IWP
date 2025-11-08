using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [HideInInspector] public ObjectPool Pool;

    public void ReturnToPool()
    {
        Pool.ReturnObject(this.gameObject);
    }
}
