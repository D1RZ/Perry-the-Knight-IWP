using System.Collections;
using UnityEngine;

public class BossRoomCutscenes : MonoBehaviour
{
    [SerializeField] private Transform playerEntryStandPos; // player position during boss entry cutscene
    [SerializeField] private Transform BossGatePos;
    [SerializeField] private GameObject BossGate;
    [SerializeField] private GameObject DemonSlime;
    [SerializeField] private Transform demonSlimeTargetPos;
    [SerializeField] private Transform BossFightCamPos;
    [SerializeField] private float demonMoveSpeed = 3f;
    [SerializeField] private GameObject DialogueUI;
    [SerializeField] private GameObject DemonKing;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController.Instance.PlayerInCutscene = true;
        transform.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(BossEntrySegment());
    }

    private IEnumerator BossEntrySegment()
    {
        yield return FadeManager.Instance.FadeOut();

        PlayerController.Instance.transform.position = playerEntryStandPos.position;
        PlayerController.Instance.transform.localScale = new Vector3(1,1,1); // ensures that the player is facing the right direction
        PlayerController.Instance.facingDirection = 1;
        PlayerController.Instance.rb.velocity = Vector3.zero;
        PlayerController.Instance.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        ResetPlayer();
        PlayerController.Instance.animationController.animator.Play("Idle",0,0.0f);
        CameraManager.Instance.Follow(null);

        yield return FadeManager.Instance.FadeIn();

        yield return StartCoroutine(CameraManager.Instance.MoveCameraTo(BossGatePos.position,1.5f));

        BossGate.GetComponent<Animator>().SetFloat("direction", 1);
        BossGate.GetComponent<Animator>().SetTrigger("Appear");

        yield return new WaitForSeconds(1.6f);

        DemonSlime.GetComponent<DemonSlime>().InCutscene = true;
        DemonSlime.GetComponent<Rigidbody2D>().gravityScale = 0;
        DemonSlime.gameObject.SetActive(true);
        yield return StartCoroutine(FadeManager.Instance.FadeSprite(DemonSlime.transform.GetChild(0).GetComponent<SpriteRenderer>(),1,1));
        DemonSlime.GetComponent<Rigidbody2D>().gravityScale = 1;
        BossGate.GetComponent<Animator>().SetFloat("direction", -1);
        BossGate.GetComponent<Animator>().Play("Appear", 0, 1.0f);
        BossGate.GetComponent<Animator>().ResetTrigger("Appear");

        yield return new WaitForSeconds(1.8f);

        IEnumerator camRoutine = CameraManager.Instance.MoveCameraTo(
        BossFightCamPos.position, 5f
        );

        IEnumerator slimeRoutine = MoveDemonSlimeToPoint();

        // start both
        StartCoroutine(camRoutine);
        StartCoroutine(slimeRoutine);

        // wait until both are done
        yield return camRoutine;
        yield return slimeRoutine;

        DialogueUI.SetActive(true);
        yield return StartCoroutine(MoveDialogueUI(Vector3.zero,1));

        yield return new WaitForSeconds(0.3f);

        DialogueManager.Instance.ShowDialogue("Where is the Demon King?!");

        yield return new WaitUntil(() => !DialogueManager.Instance.TypeInProgress);

        DialogueManager.Instance.dialogueName.text = "SLIME";

        yield return new WaitForSeconds(0.1f);

        DialogueManager.Instance.ShowDialogue("If you want answers, fight me first, child.");

        yield return new WaitUntil(() => !DialogueManager.Instance.TypeInProgress);

        DialogueManager.Instance.dialogueName.text = "PLAYER";

        yield return new WaitForSeconds(0.1f);

        DialogueManager.Instance.ShowDialogue("Fine, Let's end this.");

        yield return new WaitUntil(() => !DialogueManager.Instance.TypeInProgress);

        yield return StartCoroutine(MoveDialogueUI(new Vector3(0, -5, 0), 1));

        RoomManager.Instance.CurrentLeftBossBarrier.SetActive(true);
        RoomManager.Instance.CurrentRightBossBarrier.SetActive(true);

        DemonSlime.GetComponent<DemonSlime>().HealthBar.SetActive(true);
        PlayerController.Instance.disableRollCollider = false;
        PlayerController.Instance.PlayerInCutscene = false;
        DemonSlime.GetComponent<DemonSlime>().InCutscene = false;
    }

    public IEnumerator BossTransitionSegment()
    {
        yield return new WaitForSeconds(0.5f);

        yield return FadeManager.Instance.FadeOut();

        DemonSlime.GetComponent<DemonSlime>().HealthBar.SetActive(false);

        PlayerController.Instance.PlayerInCutscene = true;

        DemonKing.GetComponent<Enemy>().InCutscene = true;
        DemonKing.GetComponent<Enemy>().spriteRenderer.GetComponent<Animator>().speed = 0;
        DemonKing.transform.position = new Vector3(demonSlimeTargetPos.position.x, DemonKing.transform.position.y, DemonKing.transform.position.z);
        DemonKing.SetActive(true);

        PlayerController.Instance.transform.position = playerEntryStandPos.position;
        PlayerController.Instance.transform.localScale = new Vector3(1, 1, 1); // ensures that the player is facing the right direction
        PlayerController.Instance.facingDirection = 1;
        PlayerController.Instance.rb.velocity = Vector3.zero;
        PlayerController.Instance.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        ResetPlayer();
        PlayerController.Instance.animationController.animator.Play("Idle", 0, 0.0f);

        yield return FadeManager.Instance.FadeIn();

        DialogueManager.Instance.dialogueText.text = "";

        yield return StartCoroutine(MoveDialogueUI(Vector3.zero, 1));

        DialogueManager.Instance.ShowDialogue("I have won, now where is the Demon King.");

        yield return new WaitUntil(() => !DialogueManager.Instance.TypeInProgress);

        DemonKing.GetComponent<Enemy>().spriteRenderer.GetComponent<Animator>().speed = 1;


    }

    private void ResetPlayer()
    {
        PlayerController.Instance.GetComponent<Animator>().SetBool("IsMoving", false);
        PlayerController.Instance.GetComponent<Animator>().SetFloat("yVelocity", 0);
        PlayerController.Instance.GetComponent<Animator>().SetBool("InAir", false);
    }

    private IEnumerator MoveDemonSlimeToPoint()
    {
        Rigidbody2D rb = DemonSlime.GetComponent<Rigidbody2D>();
        Transform slimeTransform = DemonSlime.transform;

        float direction = Mathf.Sign(
            demonSlimeTargetPos.position.x - slimeTransform.position.x
        );

        PlayerController.Instance.rb.constraints = RigidbodyConstraints2D.None;

        PlayerController.Instance.rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        DemonSlime.transform.GetChild(0).GetComponent<Animator>().Play("Slime_Move", 0, 0.0f);

        rb.velocity = new Vector2(-demonMoveSpeed, 0f);

        // Wait until close enough on X axis
        yield return new WaitUntil(() =>
            Mathf.Abs(slimeTransform.position.x - demonSlimeTargetPos.position.x) < 0.1f
        );

        // Snap + stop
        rb.velocity = Vector2.zero;

        DemonSlime.transform.GetChild(0).GetComponent<Animator>().Play("Slime_Idle", 0, 0.0f);

        slimeTransform.position = new Vector3(
            demonSlimeTargetPos.position.x,
            slimeTransform.position.y,
            slimeTransform.position.z
        );
    }

    public IEnumerator MoveDialogueUI(Vector3 targetPos, float duration)
    {
        Vector3 startPos = DialogueUI.transform.localPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            DialogueUI.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        DialogueUI.transform.localPosition = targetPos;
    }

}