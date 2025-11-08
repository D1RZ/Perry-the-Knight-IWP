using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [SerializeField] private List<ObjectPool> pools = new List<ObjectPool>();
    private Dictionary<string, ObjectPool> poolDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        poolDictionary = new Dictionary<string, ObjectPool>();
        foreach (var pool in pools)
        {
            pool.Initialize(transform);
            poolDictionary.Add(poolPrefabName(pool), pool);
        }
    }

    private string poolPrefabName(ObjectPool pool)
    {
        var field = typeof(ObjectPool).GetField("prefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var prefab = (GameObject)field.GetValue(pool);
        return prefab.name;
    }

    public GameObject Spawn(string prefabName, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.TryGetValue(prefabName, out var pool))
        {
            Debug.LogWarning($"No pool found for prefab: {prefabName}");
            return null;
        }

        return pool.GetObject(position, rotation);
    }

    public void ReturnToPool(GameObject obj)
    {
        var pooledObj = obj.GetComponent<PooledObject>();
        if (pooledObj != null)
            pooledObj.ReturnToPool();
        else
            Destroy(obj);
    }

}
