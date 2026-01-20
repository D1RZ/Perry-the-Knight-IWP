using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Coroutine FadeTextCoroutine;
    private bool playerInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        text.alpha = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FadeText(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FadeText(false);
            playerInRange = false;
        }
    }

    private void FadeText(bool show)
    {
        if (FadeTextCoroutine != null) StopCoroutine(FadeTextCoroutine);

        FadeTextCoroutine = StartCoroutine(FadeTextRoutine(show));
    }

    private IEnumerator FadeTextRoutine(bool show)
    {
        float start = text.alpha;
        float target = show ? 1f : 0f;
        float speed = 6f;

        while (!Mathf.Approximately(text.alpha, target))
        {
            text.alpha = Mathf.MoveTowards(
                text.alpha,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }
    }

}
