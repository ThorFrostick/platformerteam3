using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class Leaderboard : MonoBehaviour
{
    //We will use a list of PlayerScores to write data to the JSON file.
    private Scores writingScores;

    //Use another list to store the PlayerScores we read from the file.
    private Scores readingScores;

    //Define the path we want to read from and write to
    //private string path = Path.Combine(Application.persistentDataPath + "leaderboard.json");
    public TextAsset file;

    //This will be externally updated when the level ends, and will be used to add a new score to the leaderboard.
    [HideInInspector]
    public int coins;

    //Get the Text UI we will use to display the leaderboard.
    [SerializeField]
    private TextMeshProUGUI display;

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
        AddScore(readingScores, coins);
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

    public void AddScore(Scores scores, int currentCoins)
    {
        //Create a new score based on the coins recently acquired
        PlayerScores newScore = new PlayerScores() { coins = currentCoins };

        //Give the player an achievment if they got at least -- 50 -- coins
        if(newScore.coins >= 50)
        {
            newScore.achievment = true;
        }
        else
        {
            newScore.achievment = false;
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
        sortedList.scores.Sort((b, a) => a.coins.CompareTo(b.coins));
        for(int i = 0; i < sortedList.scores.Count; i++)
        {
            textDisplay += $"{i + 1}: {sortedList.scores[i].coins}";

            if (sortedList.scores[i].achievment)
            {
                textDisplay += $"  Coin Hunter";
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
        public int coins;
        public bool achievment;
    }

    [Serializable]
    public class Scores
    {
        public List<PlayerScores> scores;
    }
}
