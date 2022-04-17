using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Camera _cam;

    private void Awake()
    {
        Instance = this;
        _cam = GetComponent<Camera>();
    }

    public void doShake(float duration)
    {
        StartCoroutine(Shake(duration));
    }

    public IEnumerator Shake(float duration)
    {
        Vector3 originalPos = _cam.transform.position;

        float eslapsedTime = 0f;

        while (eslapsedTime < duration)
        {
            float xOffset = Random.Range(-0.1f + originalPos.x, 0.1f + originalPos.x);
            float yOffset = Random.Range(-0.1f + originalPos.y, 0.1f + originalPos.y);

            transform.localPosition = new Vector3(xOffset, yOffset, originalPos.z - 1); // z = 0 will blacksccreen everything

            eslapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
