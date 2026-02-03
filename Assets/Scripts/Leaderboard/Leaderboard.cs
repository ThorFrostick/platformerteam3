using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Instance { get; private set; }
    
    //We will use a list of PlayerScores to write data to the JSON file.
    private Scores writingScores;

    //Use another list to store the PlayerScores we read from the file.
    private Scores readingScores;

    //Define the path we want to read from and write to
    //private string path = Path.Combine(Application.persistentDataPath + "leaderboard.json");
    public TextAsset file;

    //This will be externally updated when the level ends, and will be used to add a new score to the leaderboard.
    [HideInInspector]
    public int score;

    [HideInInspector]
    public float time;

    [HideInInspector]
    public int companions;

    //Get the Text UI we will use to display the leaderboard.
    [SerializeField]
    private TextMeshProUGUI display;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        GameManager.Instance.UpdateNewPlayer();
    }

    public void Start()
    {
        LoadFile();

        DisplayLeaderboard();
        
        WriteFile(readingScores.scores);
    }

    /// <summary>
    /// Load JSON data from a specified path.
    /// </summary>
    public void LoadFile()
    {
        //Get all the data from the JSON file.
        string jsonText = File.ReadAllText(AssetDatabase.GetAssetPath(file));

        //Translate the JSON string into our list of readed Scores.
        readingScores = JsonUtility.FromJson<Scores>(jsonText);

        //Add our new score count to the list of read-in scores.
        AddScore(readingScores, score);
    }

    /// <summary>
    /// Write data to our JSON file.
    /// </summary>
    public void WriteFile(List<PlayerScores> players)
    {
        //Pass our updated readScores into our list for writing.
        writingScores = new Scores();
        writingScores.scores = players;

        //Translate our entire updated list back into JSON format.
        string json = JsonUtility.ToJson(writingScores, true);

        //Write all the updated data to the JSON file.
        File.WriteAllText(AssetDatabase.GetAssetPath(file), json);
    }

    public void AddScore(Scores scores, int currentScore)
    {
        //Create a new score based on the coins recently acquired
        PlayerScores newScore = new PlayerScores() { score = currentScore };

        //Give the player the Scoremaster achievement if they get at least 5000 score.
        if(newScore.score >= 5000)
        {
            newScore.totalScore = true;
        }
        else
        {
            newScore.totalScore = false;
        }

        //If the player ran for longer than 30 seconds, give them the Far Runner achievement,
        if (time >= 30.0f)
        {
            newScore.farRunner = true;
        }
        else
        {
            newScore.farRunner = false;
        }

        //If the player got at least 3 friends, give them the Companion achievement.
        if(companions >= 2)
        {
          newScore.companion = true;
        }
        else
        {
          newScore.companion = false;
        }

        //Add the new score to the list of scores we have loaded in
        scores.scores.Add(newScore);
    }

    /// <summary>
    /// Display our leaderboard on screen for the user.
    /// </summary>
    public void DisplayLeaderboard()
    {
        string textDisplay = "High Scores:\n\n";

        //Loop through our updated read-list and add the scores to the display.
        Scores sortedList = readingScores;
        sortedList.scores.Sort((b, a) => a.score.CompareTo(b.score));
        for(int i = 0; i < sortedList.scores.Count; i++)
        {
            textDisplay += $"{i + 1}: {sortedList.scores[i].score}";

            if (sortedList.scores[i].totalScore)
            {
                // -- Change this to icon for Scoremaster achievement --
                textDisplay += $" Scoremaster";
            }

            if (sortedList.scores[i].farRunner)
            {
                // -- Change this to the icon for Far Runner achievement --
                textDisplay += $" Far Runner";
            }

            if (sortedList.scores[i].companion)
            {
                // -- Change this to the icon for the Companion achievement -- 
                textDisplay += $" Companion";
            }

            textDisplay += "\n";
        }

        //Upload our string to the display.
        display.text = textDisplay;
    }

    /// <summary>
    /// This class is the format we will use for uploading data to/from JSON files.
    /// </summary>
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
