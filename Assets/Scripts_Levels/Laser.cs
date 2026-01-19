using UnityEngine;

public class Laser : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        //First, check if the Player is what we are colliding with.
        if(collision.gameObject.tag == "Player")
        {
            //If we are colliding with the Player, 
            // -- TESTING PURPOSES ONLY --
            //reset the Player to the beginning of the level.
            collision.gameObject.transform.position = new Vector3(-0.18f, 0f, 8.81f);
        }
    }
}
