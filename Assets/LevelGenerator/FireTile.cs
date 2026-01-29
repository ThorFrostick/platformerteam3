using Ball;
using UnityEngine;

public class FireTile : TileBase
{
    [Header("Always On")]
    public GameObject flameVfx;
    public Collider damageTrigger;

    [Header("Kill Settings")]
    public bool killInstant = true;

    private GameObject player;

    private void Reset()
    {
        type = TileType.Fire;
    }

    void Awake()
    {
        // Check the flame vfx
        if (flameVfx != null) flameVfx.SetActive(true);

        // find trigger if it's null
        if (damageTrigger == null)
        {
            var trig = GetComponentInChildren<FireTile>();
            if (trig != null) damageTrigger = trig.GetComponent<Collider>();
        }

        if (damageTrigger != null) damageTrigger.enabled = true;
    }

    // Use in FireTrigger when player enter
    public void NotifyPlayerEnter(GameObject player)
    {
        if (killInstant)
        {
            KillPlayer(player);
            return;
        }
    }

    // Use in FireTrigger when player exit
    public void NotifyPlayerExit(GameObject player)
    { 
        if (this.player == player)
        {
            this.player = null;
        }
    }

    // Kill the player when enter
    void KillPlayer(GameObject player)
    {
        //If we are colliding with the Player, display a Game Over message.
        // -- TESTING PURPOSES ONLY --
        //ScoreManager.Instance.ResetScore();
        Debug.Log("Killed by fire tile");
        player.GetComponent<Player>()?.Die();

    }

    void OnDestroy()
    {
        GameObject.Destroy(this);
        this.player = null;
    }
}

