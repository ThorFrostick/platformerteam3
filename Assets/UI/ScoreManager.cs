using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private int score = 0;
    public int Score => score;

    // Score change event for UI
    public event Action<int> OnScoreChanged;

    [Header("Time-based Scoring")]
    [SerializeField] private bool countTimeScore = true;

    [SerializeField] private float baseRate = 1f;      // base point/sec
    [SerializeField] private float accelPerSecond = 0.2f; // acceleration speed
    [SerializeField] private float maxRate = 10f;        // max point/sec
    [SerializeField] private float warmupSeconds = 0f;   // warmup time

    private float elapsedTime = 0f;
    private float accumulator = 0f;
    private float lastRate = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ResetScore();

    }

    private void Update()
    {
        if (countTimeScore)
            AddTimeScore();
    }

    public void AddScore(int amount)
    {
        if (amount == 0) return;
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

    public int GetScore() { return score; }

    void AddTimeScore()
    {
        elapsedTime += Time.deltaTime;

        // warmup rate
        float t = Mathf.Max(0f, elapsedTime - warmupSeconds);

        // rate = base + accel * t
        float rate = baseRate + accelPerSecond * t;
        rate = Mathf.Min(rate, maxRate);

        accumulator += rate * Time.deltaTime;

        if (accumulator >= 1f)
        {
            int pointsToAdd = Mathf.FloorToInt(accumulator);
            accumulator -= pointsToAdd;
            AddScore(pointsToAdd); // use AddScore to trigger OnScoreChanged
        }

        lastRate = rate;
    }

    public void ResetScore()
    {
        score = 0;
        elapsedTime = 0f;
        accumulator = 0f;
        lastRate = 0f;
        OnScoreChanged?.Invoke(score);
    }

    public void SetTimeScoring(bool enabled)
    {
        countTimeScore = enabled;
    }
}
