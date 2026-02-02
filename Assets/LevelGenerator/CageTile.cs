using UnityEngine;
using System.Collections;

public class CageTile : TileBase
{
    [Header("Rise Settings")]
    public GameObject cageVisual;
    public bool triggered = false;
    public Transform visual;

    private float startYOffset = -0.6f;

    private float endYOffset = 0.8f;

    private float riseTime = 0.5f;  // second

    private bool riseOnSpawn = true;

    private bool hasRisen = false;
    private Coroutine routine;

    private void Reset()
    {
        type = TileType.cage;
    }

    void Awake()
    {
        if (visual == null) visual = transform;
    }

    void Start()
    {
        Vector3 p = visual.localPosition;
        visual.localPosition = new Vector3(p.x, startYOffset, p.z);

        if (riseOnSpawn)
            BeginRise();
    }

    void BeginRise()
    {
        if (hasRisen) return;
        hasRisen = true;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(RiseRoutine());
    }

    IEnumerator RiseRoutine()
    {
        Vector3 start = visual.localPosition;
        Vector3 end = new Vector3(start.x, endYOffset, start.z);

        float t = 0f;
        while (t < riseTime)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / riseTime);
            // easeOut
            float eased = 1f - Mathf.Pow(1f - u, 3f);

            visual.localPosition = Vector3.Lerp(start, end, eased);
            yield return null;
        }

        visual.localPosition = end;
        routine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if(!other.CompareTag("Player")) return;

        triggered = true;

        if (cageVisual == null) return;
        cageVisual.SetActive(false);

        other.GetComponent<Player>()?.AddExtraLife();
        
    }
}
