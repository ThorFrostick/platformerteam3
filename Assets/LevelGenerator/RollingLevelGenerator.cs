using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileEntry
{
    public TileType type;
    public GameObject prefab;
}

public class RollingLevelGenerator : MonoBehaviour
{
    [Header("Grid")]
    public int columns = 5;          // colume length
    public float tileSize = 1.5f;    // tile distance
    public int keptRows = 7;         // row on platforma

    [Header("Player + Progress")]
    public Transform player;
    public int rowsBehindPlayer = 1;

    [Header("Spawn / Drop Animation")]
    public float spawnHeight = 8f;   // New colume spawn height
    public float dropTime = 0.35f;   // Colume drop time

    [Header("Tile Prefabs")]
    public List<TileEntry> tiles;
    public Transform tilesParent;

    [Header("Generate Type Weights")]
    // Random generate weight
    public float wEmpty = 0.25f;
    public float wNormal = 0.55f;
    public float wJump = 0.12f;
    public float wLaser = 0.08f;

    [Header("Tpye limit")]
    [Range(0f, 1f)] public float laserMaxPerRowRatio = 0.2f; // Only One laser in a row
    public bool forbidLaserOnSafePath = true;
    public bool forbidLaserAdjacentToSafePath = false;

    [Header("Random")]
    public int seed = 0;
    public bool useRandomSeed = true;

    // internal
    private System.Random rng;
    private Dictionary<TileType, GameObject> prefabMap;

    // Row management
    private int nextRowIndex = 0;
    private int safeCol = 2;

    // Save each row data, use for delete
    private readonly Queue<GameObject> rowRoots = new();

    void Awake()
    {
        prefabMap = new Dictionary<TileType, GameObject>();
        foreach (var e in tiles)
            if (e.prefab != null) prefabMap[e.type] = e.prefab;

        if (useRandomSeed) seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        rng = new System.Random(seed);
    }

    void Start()
    {
        // Generate few rows at the begining of the game
        for (int i = 0; i < keptRows; i++)
            GenerateRowInstant(nextRowIndex++);
    }

    void Update()
    {
        if (!player) return;

        // Check the row player currently on, use the depth value
        int playerRow = Mathf.FloorToInt(player.position.z / tileSize);

        // Keep constant number of rows on the platform
        int desiredFirstRow = Mathf.Max(0, playerRow - rowsBehindPlayer);
        int desiredLastRowExclusive = desiredFirstRow + keptRows;

        // If the current rows didn't reach the constant number of row, generate new row
        while (nextRowIndex < desiredLastRowExclusive)
        {
            GenerateRowDrop(nextRowIndex++);
        }

        // Delete the old row if current row number is bigger than keptRows number
        while (rowRoots.Count > keptRows)
        {
            var old = rowRoots.Dequeue();
            Destroy(old);
        }
    }

    // Generate one row directly on the platform
    void GenerateRowInstant(int rowIndex)
    {
        var rowRoot = new GameObject($"Row_{rowIndex}");
        rowRoot.transform.SetParent(tilesParent, worldPositionStays: false);
        rowRoot.transform.position = new Vector3(0f, 0f, rowIndex * tileSize);

        var types = BuildRowTypes();
        for (int col = 0; col < columns; col++)
        {
            if (types[col] == TileType.Empty) continue;
            SpawnTile(types[col], rowIndex, col, rowRoot.transform, yOffset: 0f);
        }

        rowRoots.Enqueue(rowRoot);
    }

    // Generate one row from the sky  ------------------------------------------------------ Need fix the height
    void GenerateRowDrop(int rowIndex)
    {
        var rowRoot = new GameObject($"Row_{rowIndex}");
        rowRoot.transform.SetParent(tilesParent, worldPositionStays: false);

        // Set the row on the sky
        Vector3 targetPos = new Vector3(0f, 0f, rowIndex * tileSize);
        rowRoot.transform.position = targetPos + Vector3.up * spawnHeight;

        Debug.Log($"[GenerateRowDrop] after set sky pos: row={rowIndex}, pos={rowRoot.transform.position}, target={targetPos}, spawnHeight={spawnHeight}");


        var types = BuildRowTypes();
        for (int col = 0; col < columns; col++)
        {
            if (types[col] == TileType.Empty) continue;
            SpawnTile(types[col], rowIndex, col, rowRoot.transform, yOffset: 0f);
        }


        // drop animation
        StartCoroutine(DropRow(rowRoot.transform, targetPos, dropTime));

        rowRoots.Enqueue(rowRoot);
    }

    // Drop passed row
    IEnumerator DropRow(Transform t, Vector3 targetPos, float time)
    {
        Debug.Log($"[DropRow] start pos={t.position}, target={targetPos}");

        Vector3 start = t.position;
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float u = Mathf.Clamp01(elapsed / time);

            // easeOut
            float eased = 1f - Mathf.Pow(1f - u, 3f);

            t.position = Vector3.Lerp(start, targetPos, eased);
            yield return null;
        }

        t.position = targetPos;
    }

    // Random generate a row of different tiles
    TileType[] BuildRowTypes()
    {
        safeCol = MoveSafeCol(safeCol);

        TileType[] row = new TileType[columns];
        for (int c = 0; c < columns; c++)
            row[c] = WeightedPick();

        // Make sure there is a safe path
        if (row[safeCol] == TileType.Empty) row[safeCol] = TileType.Normal;
        if (forbidLaserOnSafePath && row[safeCol] == TileType.Laser) row[safeCol] = TileType.Normal;

        if (forbidLaserAdjacentToSafePath)
        {
            int left = safeCol - 1;
            int right = safeCol + 1;
            if (left >= 0 && row[left] == TileType.Laser) row[left] = TileType.Empty;
            if (right < columns && row[right] == TileType.Laser) row[right] = TileType.Empty;
        }

        // Limited the number of laser tile
        int maxLaser = Mathf.FloorToInt(columns * laserMaxPerRowRatio);
        maxLaser = Mathf.Clamp(maxLaser, 0, 2);

        while (Count(row, TileType.Laser) > maxLaser)
        {
            for (int c = 0; c < columns && Count(row, TileType.Laser) > maxLaser; c++)
            {
                if (row[c] == TileType.Laser && c != safeCol)
                    row[c] = (rng.NextDouble() < 0.6) ? TileType.Empty : TileType.Normal;
            }
        }

        return row;
    }

    // Spawn a single tile at 000
    void SpawnTile(TileType type, int rowIndex, int col, Transform rowRoot, float yOffset)
    {
        if (!prefabMap.TryGetValue(type, out var prefab) || prefab == null)
        {
            Debug.LogWarning($"No prefab set for TileType: {type}");
            return;
        }

        Debug.Log($"[SpawnTile] type={type} row={rowIndex} col={col} yOffset={yOffset} rowRootY={rowRoot.position.y}");

        float x = (col - (columns - 1) * 0.5f) * tileSize;
        float zLocal = 0f; // z = rowIndex * tileSize
        Vector3 localPos = new Vector3(x, yOffset, zLocal);

        GameObject go = Instantiate(prefab, rowRoot);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.identity;

        TileBase tile = go.GetComponent<TileBase>();
        if (tile == null)
        {
            Debug.LogError($"Prefab {prefab.name} has no TileBase on root.");
            return;
        }

        tile.Init(rowIndex, col, tileSize);
    }

    int MoveSafeCol(int current)
    {
        int roll = rng.Next(0, 100);
        int step = 0;
        if (roll < 25) step = -1;
        else if (roll < 50) step = +1;
        return Mathf.Clamp(current + step, 0, columns - 1);
    }

    TileType WeightedPick()
    {
        double total = wEmpty + wNormal + wJump + wLaser;
        double r = rng.NextDouble() * total;

        if (r < wEmpty) return TileType.Empty;
        r -= wEmpty;

        if (r < wNormal) return TileType.Normal;
        r -= wNormal;

        if (r < wJump) return TileType.Jump;
        return TileType.Laser;
    }

    int Count(TileType[] row, TileType t)
    {
        int n = 0;
        for (int i = 0; i < row.Length; i++)
            if (row[i] == t) n++;
        return n;
    }
}
