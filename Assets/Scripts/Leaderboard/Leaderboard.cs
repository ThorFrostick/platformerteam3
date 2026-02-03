using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI display;

    public int maxEntriesToShow = 10;

    public string fileName = "leaderboard.json";

    [HideInInspector] public int score;
    [HideInInspector] public float time;
    [HideInInspector] public int companions;

    private Scores scoresData = new Scores() { scores = new List<PlayerScores>() };

    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Displays the leaderboard when the player dies and updates it with the final run data
    public void ShowOnDeath(int finalScore, float finalTime, int finalCompanions)
    {
        score = finalScore;
        time = finalTime;
        companions = finalCompanions;

        LoadFile();
        AddScore(scoresData, score, time, companions);
        SaveFile();
        DisplayLeaderboard();
    }

    // Load the leader board data file
    void LoadFile()
    {
        if (!File.Exists(FilePath))
        {
            scoresData = new Scores() { scores = new List<PlayerScores>() };
            SaveFile();
            return;
        }

        try
        {
            string json = File.ReadAllText(FilePath);
            var loaded = JsonUtility.FromJson<Scores>(json);

            if (loaded == null || loaded.scores == null)
                scoresData = new Scores() { scores = new List<PlayerScores>() };
            else
                scoresData = loaded;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Leaderboard] Failed to load: {e.Message}");
            scoresData = new Scores() { scores = new List<PlayerScores>() };
        }
    }

    // Save the player score data
    void SaveFile()
    {
        try
        {
            string json = JsonUtility.ToJson(scoresData, true);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Leaderboard] Failed to save: {e.Message}");
        }
    }

    // Add new score info to the leader board, also check if the player has unlocked any achievements
    void AddScore(Scores scores, int currentScore, float runTime, int comp)
    {
        if (scores.scores == null) scores.scores = new List<PlayerScores>();

        PlayerScores newScore = new PlayerScores()
        {
            score = currentScore,
            totalScore = (currentScore >= 5000),
            farRunner = (runTime >= 20.0f),
            companion = (comp >= 2)
        };

        scores.scores.Add(newScore);
    }

    // Display the leader board
    void DisplayLeaderboard()
    {
        if (display == null) return;

        scoresData.scores.Sort((b, a) => a.score.CompareTo(b.score));

        string textDisplay = "High Scores:\n\n";

        int count = Mathf.Min(maxEntriesToShow, scoresData.scores.Count);
        for (int i = 0; i < count; i++)
        {
            var s = scoresData.scores[i];
            textDisplay += $"{i + 1}: {s.score}";

            if (s.totalScore) textDisplay += $" <sprite index=2>";
            if (s.farRunner) textDisplay += $" <sprite index=1>";
            if (s.companion) textDisplay += $" <sprite index=0>";

            textDisplay += "\n";
        }

        display.text = textDisplay;
    }

    [Serializable]
    public class PlayerScores
    {
        public int score;
        public bool totalScore;
        public bool farRunner;
        public bool companion;
    }

    [Serializable]
    public class Scores
    {
        public List<PlayerScores> scores;
    }
}