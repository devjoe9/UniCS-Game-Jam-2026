using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject camObj;

    private CinemachineCamera cineCam;
    private float defaultOrthSize;
    private CamShake camShake;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cineCam = camObj.GetComponent<CinemachineCamera>();
        defaultOrthSize = cineCam.Lens.OrthographicSize;
        camShake = FindAnyObjectByType<CamShake>();
    }

    public void ChangeOrthSize(float mult, float duration, bool toDefault)
    {
        float targetOrthSize = toDefault ? defaultOrthSize : defaultOrthSize * mult;
        StartCoroutine(SmoothOrthSizeChange(targetOrthSize, duration));
    }

    IEnumerator SmoothOrthSizeChange(float targetOrthSize, float duration)
    {
        float startOrthSize = cineCam.Lens.OrthographicSize;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;

            float t = timeElapsed / duration;
            float newOrthSize = Mathf.Lerp(startOrthSize, targetOrthSize, t);

            cineCam.Lens.OrthographicSize = newOrthSize;

            yield return null;
        }

        cineCam.Lens.OrthographicSize = targetOrthSize;
    }

    public void ShakeCamera(float amplitude, float frequency, bool isShaking)
    {
        if (isShaking)
        {
            camShake.StartShake(amplitude, frequency);
            return;
        }
        camShake.StopShake();
    }
}
