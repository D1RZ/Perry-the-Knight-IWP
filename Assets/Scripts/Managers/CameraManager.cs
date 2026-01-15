using Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public CinemachineVirtualCamera cam;
    private CinemachineFramingTransposer framing;
    private CinemachineBasicMultiChannelPerlin perlin;

    private Transform defaultTarget;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        framing = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        perlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        defaultTarget = cam.Follow;
    }

    public void Follow(Transform target)
    {
        cam.Follow = target;
    }

    public void ResetFollow()
    {
        cam.Follow = defaultTarget;
    }

    public void Shake(float duration, float intensity)
    {
        StartCoroutine(DoShake(duration, intensity));
    }

    private IEnumerator DoShake(float duration, float intensity)
    {
        perlin.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(duration);
        perlin.m_AmplitudeGain = 0f;
    }

    public IEnumerator MoveCameraTo(Vector3 targetPos, float duration)
    {
        Vector3 startPos = cam.transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        cam.transform.position = targetPos;
    }

}
