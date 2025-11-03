using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // or .HighDefinition if you’re on HDRP

public class PostProcessController : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;

    private Vignette vignette;

    private void Start()
    {
        if (globalVolume != null && globalVolume.profile.TryGet(out vignette))
        {
            // optional: set initial state
            vignette.active = true;
            vignette.intensity.value = 0f; // start off
        }
        else
        {
            Debug.LogWarning("No Vignette override found on the Global Volume!");
        }
    }

    public void EnableVignette(float targetIntensity = 0.4f)
    {
        if (vignette != null)
        {
            vignette.intensity.value = targetIntensity;
        }
    }

    public void DisableVignette()
    {
        if (vignette != null)
        {
            vignette.intensity.value = 0f;
            vignette.active = false;
        }
    }

    // optional smooth fade effect
    public IEnumerator FadeVignette(float target, float duration)
    {
        if (vignette == null) yield break;

        float start = vignette.intensity.value;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            vignette.intensity.value = Mathf.Lerp(start, target, t);
            yield return null;
        }
    }
}
