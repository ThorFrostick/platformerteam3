using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlameTrigger : MonoBehaviour
{
    private FireTile tile;

    void Awake()
    {
        tile = GetComponentInParent<FireTile>();

        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (tile == null) return;
        if (other.CompareTag("Player"))
            tile.NotifyPlayerEnter(other.gameObject);
    }


    /*
    // In case the player has multiple lives
    void OnTriggerExit(Collider other)
    {
        if (tile == null) return;
        if (other.CompareTag("Player"))
            tile.NotifyPlayerExit(other.gameObject);
    }
    */
}

