using UnityEngine;

public class Room : MonoBehaviour
{
    public Collider2D cameraBounds;
    public Transform enemiesParent;
    public Transform playerSpawnPos;

    public void OnEnterRoom()
    {
        //foreach (Enemy enemy in enemiesParent.GetComponentsInChildren<Enemy>(true))
        //{
        //    Debug.Log("Reset Enemy!");
        //    enemy.ResetEnemy();
        //    enemy.gameObject.SetActive(true);
        //}

        transform.gameObject.SetActive(true);
    }

    public void OnExitRoom()
    {
        transform.gameObject.SetActive(false);
    }

}
