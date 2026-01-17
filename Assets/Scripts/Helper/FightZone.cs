using System.Collections.Generic;
using UnityEngine;

public class FightZone : MonoBehaviour
{
    private HashSet<Enemy> enemies = new HashSet<Enemy>();
    [SerializeField] private List<GameObject> lightningBarriers = new List<GameObject>();
    [SerializeField] private int TotalNoOfEnemiesInFightZone;
    private int EnemiesKilledInFightZone = 0;
    private bool FinishedAddingEnemies = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8) // aka if its the player that enters the fight zone
        {
            transform.GetComponent<Collider2D>().enabled = false;
            return;
        }

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemies.Contains(enemy)) return;

        enemies.Add(enemy);

        Debug.Log("Enemies count: " + enemies.Count + " " + TotalNoOfEnemiesInFightZone);

        if (enemies.Count == TotalNoOfEnemiesInFightZone)
        {
            Debug.Log("FINISHED ADDING ENEMIES");
            FinishedAddingEnemies = true;
            return;
        }
    }

    private void Update()
    {
        if (FinishedAddingEnemies)
        {
            int removed = enemies.RemoveWhere(enemy =>
                enemy == null || enemy.health <= 0
            );

            if (removed > 0)
            {
                Debug.Log("ENEMY KILLED FIGHT!");
                EnemiesKilledInFightZone += removed;
            }
        }

        if(EnemiesKilledInFightZone == TotalNoOfEnemiesInFightZone)
        {
            for (int i = 0; i < lightningBarriers.Count; i++)
            {
                lightningBarriers[i].SetActive(false);
                this.enabled = false;
            }
        }
    }

}