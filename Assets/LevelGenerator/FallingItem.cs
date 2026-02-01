using UnityEngine;
using System.Collections;


public class FallingItem : MonoBehaviour
{
    public bool armed = false;
    public float heightfix = 0f;

    // Drops this object to a target LOCAL position over time.
    public void DropToLocal(Vector3 targetLocalPos, float embedDepth, float dropTime)
    {
        StopAllCoroutines();
        StartCoroutine(DropLocalRoutine(targetLocalPos, embedDepth, dropTime));
    }

    // Coroutine that animates the object moving from its current
    IEnumerator DropLocalRoutine(Vector3 targetLocalPos, float embedDepth, float dropTime)
    {
        Vector3 start = transform.localPosition;
        float t = 0f;
        targetLocalPos = targetLocalPos + Vector3.up * heightfix;
        while (t < dropTime)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dropTime);
            float eased = 1f - Mathf.Pow(1f - u, 3f);
            transform.localPosition = Vector3.Lerp(start, targetLocalPos, eased);
            yield return null;
        }

        armed = true;
    }
}
