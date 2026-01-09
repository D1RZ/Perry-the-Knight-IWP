using System.Collections;
using UnityEngine;

public class PauseAnimation : MonoBehaviour
{
    [SerializeField] private GameObject ExclamationMark;
    [SerializeField] private float resumeAnimationSpeed = 1f;
    [SerializeField] private bool pauseAnimation = true;
    [SerializeField] private float pauseAnimationTime = 0.2f;
    private Animator animator;
    private Transform ExclaimationMarkPos;

    public void PausingAnimation()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0;
        if(ExclamationMark) ExclamationMark.SetActive(true);
        if(pauseAnimation) StartCoroutine(ResumeAnimation());
    }

    private IEnumerator ResumeAnimation()
    {
        yield return new WaitForSeconds(pauseAnimationTime);
        animator.speed = resumeAnimationSpeed;
        if (ExclamationMark) ExclamationMark.SetActive(false);
    }

    public void ResumingAnimation()
    {
        StartCoroutine(ResumeAnimation());
    }

    public GameObject GetExclaimationMark()
    {
        return ExclamationMark;
    }

    public void SetExclaimationMarkPos(Transform newPos)
    {
        ExclaimationMarkPos = newPos;
    }

}
