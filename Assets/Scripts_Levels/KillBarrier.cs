using UnityEngine;

public class KillBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        //First, check if the Player is what we are colliding with.
        if (collision.gameObject.tag == "Player")
        {
            //If we are colliding with the Player, display a Game Over message.
            // -- TESTING PURPOSES ONLY --
            Debug.Log("Game Over");
        }
    }
}
