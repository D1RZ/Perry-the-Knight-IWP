using System.Collections;
using TMPro;
using UnityEngine;

public class BossGate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI BossGateText;
    [SerializeField] private SpriteRenderer BossGateIcon;
    [SerializeField] private Room BossRoom;
    private Coroutine FadeTextCoroutine;
    private Coroutine FadeSpriteCoroutine;
    private bool playerInRange = false;
    private bool playerEnteredBossRoom = false;

    private void Start()
    {
        BossGateText.alpha = 0;
        Color c = BossGateIcon.color;
        c.a = 0;
        BossGateIcon.color = c;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !playerEnteredBossRoom)
        {
            FadeSprite(true);
            FadeText(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerEnteredBossRoom)
        {
            FadeSprite(false);
            FadeText(false);
            playerInRange = false;
        }
    }

    private void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            playerEnteredBossRoom = true;
            if(FadeTextCoroutine != null) StopCoroutine(FadeTextCoroutine);
            if(FadeSpriteCoroutine != null) StopCoroutine(FadeSpriteCoroutine);
            RoomManager.Instance.EnterRoom(BossRoom,false);
        }
    }

    private void FadeText(bool show)
    {
        if (FadeTextCoroutine != null) StopCoroutine(FadeTextCoroutine);

        FadeTextCoroutine = StartCoroutine(FadeTextRoutine(show));
    }

    private IEnumerator FadeTextRoutine(bool show)
    {
        float start = BossGateText.alpha;
        float target = show ? 1f : 0f;
        float speed = 6f;

        while (!Mathf.Approximately(BossGateText.alpha, target))
        {
            BossGateText.alpha = Mathf.MoveTowards(
                BossGateText.alpha,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }
    }

    private void FadeSprite(bool show)
    {
        if (FadeSpriteCoroutine != null) StopCoroutine(FadeSpriteCoroutine);

        FadeSpriteCoroutine = StartCoroutine(FadeSpriteRoutine(show));
    }

    private IEnumerator FadeSpriteRoutine(bool show)
    {
        float start = BossGateIcon.color.a;
        float target = show ? 1f : 0f;
        float speed = 6f;

        while (!Mathf.Approximately(BossGateIcon.color.a, target))
        {
            Color c = BossGateIcon.color;
            c.a = Mathf.MoveTowards(
                c.a,
                target,
                speed * Time.deltaTime
            );
            BossGateIcon.color = c;

            yield return null;
        }
    }

}
