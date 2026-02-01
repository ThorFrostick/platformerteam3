using UnityEngine;
using System.Collections;


public class FallingItem : MonoBehaviour
{
    public bool armed = false;
    public float heightfix = 0f;
    public void DropToLocal(Vector3 targetLocalPos, float embedDepth, float dropTime)
    {
        StopAllCoroutines();
        StartCoroutine(DropLocalRoutine(targetLocalPos, embedDepth, dropTime));
    }

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

    // 用 Trigger 更稳（CollisionBox 勾 IsTrigger）
    void OnTriggerEnter(Collider other)
    {
        if (!armed) return;
        if (!other.CompareTag("Player")) return;

        var death = other.GetComponentInParent<PlayerDeath>();
        if (death != null) death.Die();
        else GameManager.Instance?.GameOver();
    }
}
