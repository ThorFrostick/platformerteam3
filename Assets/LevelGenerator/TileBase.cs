using UnityEngine;

public abstract class TileBase : MonoBehaviour
{
    [Header("Base Info")]
    public TileType type;
    public int row;
    public int col;

    // Generate tile
    public virtual void Init(int row, int col, float tileSize)
    {
        this.row = row;
        this.col = col;

        //transform.position = new Vector3(col * tileSize, 0f, row * tileSize);
    }

    public virtual void OnPlayerEnter(GameObject player) { }

    public virtual void OnPlayerExit(GameObject player) { }

}
