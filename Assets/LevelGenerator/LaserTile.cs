using Ball;
using UnityEngine;

public class LaserTile : TileBase
{
    [Header("Laser")]
    public GameObject laserVisual;
    public bool killOnTouch = true;

    private void Reset() => type = TileType.Laser;

    public override void OnPlayerEnter(GameObject player)
    {
        if (killOnTouch)
        {
            ScoreManager.Instance.ResetScore();
            Debug.Log("Game Over");
            player.GetComponent<BallController>()?.Reset();
        }
    }
}
