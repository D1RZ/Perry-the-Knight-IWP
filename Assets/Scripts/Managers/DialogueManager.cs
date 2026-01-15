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

    private void Awake()
    {
        Instance = this;
    }
     
    private void Update()
    {
        if(!isTyping && TypeInProgress)
        {
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

}
