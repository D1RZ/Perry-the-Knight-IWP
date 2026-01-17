using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public GameObject blackArrow;
    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.02f;
    private string currentFullText;
    private bool isTyping = false;
    public static DialogueManager Instance;
    private Coroutine typingCoroutine;
    public bool IsTyping => isTyping;
    public bool TypeInProgress = false;
    [SerializeField] private float amplitude = 0.15f; // world units
    [SerializeField] private float speed = 3f;
    private Vector3 startPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        startPos = blackArrow.transform.localPosition;
    }

    private void Update()
    {
        if (isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            FinishTypingInstantly();
            return;
        }

        if (!isTyping && TypeInProgress)
        {
            float offset = -Mathf.Abs(Mathf.Sin(Time.time * speed)) * amplitude;
            blackArrow.transform.localPosition = startPos + Vector3.right * offset;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TypeInProgress = false;
                blackArrow.SetActive(false);
            }
        }
    }

    public void ShowDialogue(string line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        TypeInProgress = true;
        dialogueText.text = "";

        currentFullText = line;
        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        blackArrow.SetActive(true);
        dialogueText.text = currentFullText;
        isTyping = false;
    }

    private void FinishTypingInstantly()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = currentFullText;
        isTyping = false;

        blackArrow.SetActive(true);
    }
}
