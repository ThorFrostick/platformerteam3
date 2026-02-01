using System.Collections;
using UnityEngine;

public class ItemDroppingTile : TileBase
{
    [Header("Item Prefabs (must have DropToGroundAnimated)")]
    public GameObject[] itemPrefabs;

    [Header("Spawn")]
    public float spawnHeight = 8f;
    public Vector3 spawnOffset = Vector3.zero;

    [Header("Drop Animation")]
    public float dropTime = 0.35f;               
    public float embedDepth = 0.03f;

    private bool triggered = false;

    public BoxCollider surfaceBox;

    private void Reset()
    {
        type = TileType.ItemDropping;
    }

    void Start()
    {
        BeginTrap();
    }

    void BeginTrap()
    {
        if (triggered) return;
        triggered = true;
        SpawnItemAnimated();
    }

    void SpawnItemAnimated()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0) return;

        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        if (prefab == null) return;

        if (surfaceBox == null)
        {
            Debug.LogError("[ItemDroppingTile] surfaceBox is null. Drag Visual's BoxCollider here.");
            return;
        }

        // 1) 先把 item 实例化为 tile 的子物体（这样 local 空间稳定）
        GameObject item = Instantiate(prefab, transform);
        item.transform.localRotation = Quaternion.identity;

        // 2) 计算落点：取 surfaceBox 顶面在「tile本地空间」的 y
        // surfaceBox.bounds.max.y 是世界值，但 InverseTransformPoint 会转回 tile 本地
        float topWorldY = surfaceBox.bounds.max.y;

        // 选择一个世界点（x/z 用 tile 自己的世界位置即可）
        Vector3 topWorldPoint = new Vector3(transform.position.x, topWorldY, transform.position.z);

        // 转成 tile 本地坐标
        Vector3 endLocal = transform.InverseTransformPoint(topWorldPoint);

        // 3) 起点：在落点本地坐标上方 spawnHeight（加上可选偏移）
        Vector3 startLocal = endLocal + Vector3.up * spawnHeight + spawnOffset;
        item.transform.localPosition = startLocal;

        // 4) 播放本地下落动画
        var drop = item.GetComponent<FallingItem>();
        if (drop == null)
        {
            Debug.LogError($"[ItemDroppingTile] {prefab.name} missing DropToGroundAnimated.");
            Destroy(item);
            return;
        }

        drop.DropToLocal(endLocal, embedDepth, dropTime);
    }
}
