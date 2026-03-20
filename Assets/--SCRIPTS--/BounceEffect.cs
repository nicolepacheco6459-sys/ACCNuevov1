using System.Collections;
using UnityEngine;

public class BounceEffect : MonoBehaviour
{
    public float height = 0.5f;
    public float duration = 0.6f;

    public void StartBounce()
    {
        StopAllCoroutines();
        StartCoroutine(BounceCoroutine());
    }

    private IEnumerator BounceCoroutine()
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration; // 0..1
            // Smooth up and down using sine
            float yOffset = Mathf.Sin(t * Mathf.PI) * height;
            transform.position = new Vector3(start.x, start.y + yOffset, start.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = start;
    }
}
