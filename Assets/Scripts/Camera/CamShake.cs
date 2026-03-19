using UnityEngine;
using System.Collections;
public class CamShake : MonoBehaviour
{
    private Vector3 currentPos;

    void Start()
    {
        currentPos = transform.position;
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

            transform.localPosition = new Vector3(x, y, currentPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
    }
}
