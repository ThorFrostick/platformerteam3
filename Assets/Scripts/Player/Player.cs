using System;
using Ball;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public int lifeCnt;
    public UnityAction<int> OnLifeChanged;
    private bool dead = false;
    private BallController ball;

    private void Awake()
    {
        ball = GetComponent<BallController>();
    }

    public void AddExtraLife()
    {
        lifeCnt++;
        OnLifeChanged?.Invoke(lifeCnt);
    }
    
    public void Die()
    {
        if (ball.IsInvincible)
            return;
        lifeCnt--;
        OnLifeChanged?.Invoke(lifeCnt);
        if (lifeCnt == 0)
        {
            GameOver();
        }
        else
        {
            GetComponent<BallController>().Respawn();
        }
    }

    public void GameOver()
    {
        if (dead) return;
        dead = true;

        // Notify the GameManager to enter the death state.
        GameManager.Instance?.GameOver();

        GetComponent<BallController>().enabled = false;
    }
}
