using Unity.Hierarchy;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private int score = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Timer value
    public float baseRate = 0.1f;
    public float accelPerRate = 0.01f;
    public float maxRate = 10f;

    private float elapsedTime;
    private float accumulator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void Update()
    {
        AddTimeScore();
    }

    public void AddScore(int amount)
    {
        score += amount;
        //Debug.Log("Score: " + score);
    }

    public int GetScore() { return score; }

    void AddTimeScore()
    {
        elapsedTime += Time.deltaTime;
        float rate = baseRate + (accelPerRate * elapsedTime);
        rate = Mathf.Min(rate, maxRate);

        accumulator += rate * Time.deltaTime;
        //Debug.Log(accumulator);
        if (accumulator >= 1f)
        {
            int pointsToAdd = Mathf.FloorToInt(accumulator);
            score += pointsToAdd;
            accumulator -= pointsToAdd;
        }
    }

    public void ResetScore()
    {
        score = 0;
        elapsedTime = 0f;
        accumulator = 0f;
    }
}
