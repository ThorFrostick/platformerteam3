using Ball;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        //First, check if the Player is what we are colliding with.
        if (collision.gameObject.tag == "Player")
        {
            //If we are colliding with the Player, display the Level Complete screen.
            // -- TESTING PURPOSES ONLY --
            //Debug.Log("Level Complete");
            SceneManager.LoadScene("EndMenu");
            // collision.GetComponent<BallController>()?.ResetSpeed();
        }
    }
}
