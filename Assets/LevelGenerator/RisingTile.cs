using System.Collections;
using UnityEngine;

public class RisingTile : TileBase
{
    [Header("Rise Settings")]
    public Transform visualMesh;

    public float startYOffset = 0.0f;

    public float endYOffset = 1.0f;

    public float riseTime = 0.5f;  // second

    public bool riseOnSpawn = true;

    private bool hasRisen = false;
    private Coroutine routine;

    private void Reset()
    {
        type = TileType.Rising;
    }

    void Awake()
    {
        if (visualMesh == null) visualMesh = transform;
       // if (triggerZone != null) triggerZone.isTrigger = true;
    }

    void Start()
    {
        Vector3 p = visualMesh.localPosition;
        visualMesh.localPosition = new Vector3(p.x, startYOffset, p.z);

        if (riseOnSpawn)
            BeginRise();
    }

    /*
    public override void OnPlayerEnter(GameObject player)
    {
        if (riseOnPlayerEnter)
            BeginRise();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!riseOnPlayerEnter) return;
        if (other.CompareTag("Player"))
            BeginRise();
    }
    */

    void BeginRise()
    {
        if (hasRisen) return;
        hasRisen = true;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(RiseRoutine());
    }

    IEnumerator RiseRoutine()
    {
        Vector3 start = visualMesh.localPosition;
        Vector3 end = new Vector3(start.x, endYOffset, start.z);

        float t = 0f;
        while (t < riseTime)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / riseTime);
            // easeOut
            float eased = 1f - Mathf.Pow(1f - u, 3f);

            visualMesh.localPosition = Vector3.Lerp(start, end, eased);
            yield return null;
        }

        visualMesh.localPosition = end;
        routine = null;
    }
}
