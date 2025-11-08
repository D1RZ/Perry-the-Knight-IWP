using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 10;

    private Queue<GameObject> poolQueue;
    private Transform parentContainer;

    public void Initialize(Transform parent)
    {
        poolQueue = new Queue<GameObject>();
        parentContainer = new GameObject($"{prefab.name}_Pool").transform;
        parentContainer.SetParent(parent);

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateNewObject();
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = GameObject.Instantiate(prefab, parentContainer);
        var pooledObj = obj.GetComponent<PooledObject>();
        if (pooledObj == null)
            pooledObj = obj.AddComponent<PooledObject>();
        pooledObj.Pool = this;
        return obj;
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj = poolQueue.Count > 0 ? poolQueue.Dequeue() : CreateNewObject();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}
