using UnityEngine;

[System.Serializable]
public class LootItem
{
    public GameObject prefab;
    [Range(0f, 1f)] public float dropChance = 1f; // optional weighting
}
