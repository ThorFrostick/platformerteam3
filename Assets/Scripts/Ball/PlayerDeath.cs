using Ball;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private bool dead = false;

    public void Die()
    {
        if (dead) return;
        dead = true;

        // Notify the GameManager to enter the death state.
        GameManager.Instance?.GameOver();

        GetComponent<BallController>().enabled = false;
    }
}
