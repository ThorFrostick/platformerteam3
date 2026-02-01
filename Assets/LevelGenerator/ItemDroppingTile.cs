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

    [Header("Kill Settings")]
    public bool killInstant = true;

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

        // 1) instantiate item
        GameObject item = Instantiate(prefab, transform);
        item.transform.localRotation = Quaternion.identity;

        float topWorldY = surfaceBox.bounds.max.y;

        // Get top world point base on height
        Vector3 topWorldPoint = new Vector3(transform.position.x, topWorldY, transform.position.z);

        // transfor to local position
        Vector3 endLocal = transform.InverseTransformPoint(topWorldPoint);

        // Set start point
        Vector3 startLocal = endLocal + Vector3.up * spawnHeight + spawnOffset;
        item.transform.localPosition = startLocal;

        // play dropping animation on local
        var drop = item.GetComponent<FallingItem>();
        if (drop == null)
        {
            Debug.LogError($"[ItemDroppingTile] {prefab.name} missing DropToGroundAnimated.");
            Destroy(item);
            return;
        }

        drop.DropToLocal(endLocal, embedDepth, dropTime);
    }

    public void NotifyPlayerEnter(GameObject player)
    {
        if (killInstant)
        {
            KillPlayer(player);
            return;
        }
    }

    // Kill the player when enter
    void KillPlayer(GameObject player)
    {
        //If we are colliding with the Player, display a Game Over message.
        // -- TESTING PURPOSES ONLY --
        //ScoreManager.Instance.ResetScore();
        Debug.Log("Killed by fire tile");
        player.GetComponent<PlayerDeath>()?.Die();

    }
}
