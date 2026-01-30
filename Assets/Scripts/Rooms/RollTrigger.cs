using UnityEngine;

public class RollTrigger : MonoBehaviour
{
    private float rollTimer = 0;
    private bool playerIsRolling = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController.Instance.disableRollCollider = true;
    }

    private void Update()
    {
        if(PlayerController.Instance.disableRollCollider && PlayerController.Instance.GetIsRolling() && !playerIsRolling)
        {
            rollTimer = 0;
            playerIsRolling = true;
        }

        if(playerIsRolling)
        {
            Debug.Log("IS ROLLING");

            rollTimer += Time.deltaTime;

            if(rollTimer > 0.7f)
            {
                PlayerController.Instance.disableRollCollider = false;
                PlayerController.Instance.GetComponent<Collider2D>().enabled = true;
                rollTimer = 0;
                playerIsRolling = false;
            }
        }

        if(Vector2.Distance(PlayerController.Instance.transform.position,transform.position) > 4.5f)
        {
           if(PlayerController.Instance.disableRollCollider) PlayerController.Instance.disableRollCollider = false;
            PlayerController.Instance.transform.GetComponent<Collider2D>().enabled = true;
        }
    }

}
