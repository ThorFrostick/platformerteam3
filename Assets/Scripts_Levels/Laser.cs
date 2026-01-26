using Ball;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
            ScoreManager.Instance.ResetScore();
            collision.GetComponent<PlayerDeath>()?.Die();
        }
    }
}
