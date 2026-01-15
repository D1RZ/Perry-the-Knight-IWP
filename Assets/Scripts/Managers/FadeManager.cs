using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.3f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(1f);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(0f);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }

    public IEnumerator FadeSprite(SpriteRenderer sprite, float targetAlpha, float duration)
    {
        float startAlpha = sprite.color.a;
        float time = 0f;

        Color baseColor = sprite.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            sprite.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        sprite.color = new Color(baseColor.r, baseColor.g, baseColor.b, targetAlpha);
    }

}