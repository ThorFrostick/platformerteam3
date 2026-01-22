using Unity.Hierarchy;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Timer value
    public float baseRate = 0.1f;
    public float accelPerRate = 0.15f;
    public float maxRate = 50f;

    public float elapsedTime;
    public float accumulator;

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

    void AddTimeScore()
    {
        elapsedTime += Time.deltaTime;
        float rate = baseRate + (accelPerRate * elapsedTime * elapsedTime);
        rate = Mathf.Min(rate, maxRate);

        accumulator += rate * Time.deltaTime;
        Debug.Log(accumulator);
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
