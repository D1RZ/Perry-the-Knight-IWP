using System.Collections;
using UnityEngine;

public class PauseAnimation : MonoBehaviour
{
    [SerializeField] private GameObject ExclamationMark;
    private Animator animator;

    public void PausingAnimation()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0;
        ExclamationMark.SetActive(true);
        StartCoroutine(ResumeAnimation());
    }

    private IEnumerator ResumeAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        animator.speed = 4f;
        ExclamationMark.SetActive(false);
    }

}
