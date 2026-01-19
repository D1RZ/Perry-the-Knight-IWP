using System.Collections.Generic;
using UnityEngine;

public class BuffTower : Enemy
{
    [Header("Beam")]
    [SerializeField] private Transform beamPrefab;
    [SerializeField] private Transform beamOrigin;

    [Header("Enemies In Range (Manual)")]
    [SerializeField] private List<Enemy> enemiesInRange = new List<Enemy>();
    private Dictionary<Enemy, Transform> activeBeams = new Dictionary<Enemy, Transform>();

    [Header("Heal")]
    [SerializeField] private float healPerSecond = 10f;

    public override void Start()
    {
        base.Start();

        // Initialize beams for all serialized enemies
        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            AddEnemy(enemy);
        }
    }

    public override void Update()
    {
        UpdateBeamsAndHealing();
    }

    private void AddEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        if (activeBeams.ContainsKey(enemy)) return;

        Transform beam = Instantiate(beamPrefab, beamOrigin.position, Quaternion.identity);
        activeBeams.Add(enemy, beam);

        if (enemy.BuffShield != null)
            enemy.BuffShield.SetActive(true);

        enemy.canDamage = false;
    }

    private void RemoveEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        if (activeBeams.TryGetValue(enemy, out Transform beam))
        {
            Destroy(beam.gameObject);
            activeBeams.Remove(enemy);
        }

        enemy.canDamage = true;

        if (enemy.BuffShield != null)
            enemy.BuffShield.SetActive(false);
    }


    private void UpdateBeamsAndHealing()
    {
        float healAmount = healPerSecond * Time.deltaTime;

        // Snapshot to avoid modification during iteration
        var snapshot = new List<Enemy>(enemiesInRange);

        foreach (Enemy enemy in snapshot)
        {
            if (enemy == null || enemy.health <= 0)
            {
                enemiesInRange.Remove(enemy);
                RemoveEnemy(enemy);
                continue;
            }

            // Heal over time
            if (!enemy.IsAtFullHealth)
            {
                enemy.Heal(healAmount);
            }

            // Update beam
            if (!activeBeams.TryGetValue(enemy, out Transform beam))
                continue;

            Vector2 origin = beamOrigin.position;
            Vector2 target = enemy.transform.position;
            Vector2 dir = origin - target;

            float distance = dir.magnitude;

            // Sprite faces DOWN at 0°
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            beam.rotation = Quaternion.Euler(0, 0, angle);

            beam.localScale = new Vector3(
                beam.localScale.x,
                distance * 0.096f,
                beam.localScale.z
            );
        }
    }

    public override void DeadEvent()
    {
        foreach (var pair in activeBeams)
        {
            Enemy enemy = pair.Key;
            if (enemy == null) continue;

            enemy.canDamage = true;

            if (pair.Value != null)
                Destroy(pair.Value.gameObject);

            if (enemy.BuffShield != null)
                enemy.BuffShield.SetActive(false);
        }

        activeBeams.Clear();
        enemiesInRange.Clear();
    }

}
