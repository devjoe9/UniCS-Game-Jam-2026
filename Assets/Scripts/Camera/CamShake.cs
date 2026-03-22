using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
public class CamShake : MonoBehaviour
{
    private Vector3 currentPos;
    private CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        currentPos = transform.position;
        noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
        StopShake();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        currentPos = transform.position;
    }

    public IEnumerator ShakeCam(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition += new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;

            yield return null;
        }
    }

    public void StartShake(float amplitude, float frequency)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;
    }

    public void StopShake()
    {
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }
}
