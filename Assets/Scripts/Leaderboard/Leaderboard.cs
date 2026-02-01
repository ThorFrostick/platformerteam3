using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class Leaderboard : MonoBehaviour
{
    //We will use a list of PlayerScores to store the data we get from the file.
    private Scores scores;

    
    private Scores loadedScores;

    //Define the path we want to read from and write to
    //private string path = Path.Combine(Application.persistentDataPath + "leaderboard.json");
    public TextAsset file;

    public void Start()
    {
        
        LoadFile();

        // -- Testing 
        
        
        WriteFile(loadedScores.scores);
    }

    /// <summary>
    /// Load JSON data from a specified path.
    /// </summary>
    public void LoadFile()
    {
        string jsonText = File.ReadAllText(AssetDatabase.GetAssetPath(file));

        loadedScores = JsonUtility.FromJson<Scores>(jsonText);

        //AddScore(loadedScores, 15);
    }

    /// <summary>
    /// Write data to our JSON file.
    /// </summary>
    public void WriteFile(List<PlayerScores> players)
    {
        scores = new Scores();
        scores.scores = players;

        string json = JsonUtility.ToJson(scores, true);

        File.WriteAllText(AssetDatabase.GetAssetPath(file), json);
    }

    public void AddScore(Scores scores, int currentCoins)
    {
        //Create a new score based on the coins recently acquired
        PlayerScores newScore = new PlayerScores() { coins = currentCoins };

        //Add the new score to the list of scores we have loaded in
        scores.scores.Add(newScore);
    }

    /// <summary>
    /// This class is the format we will use for uploading data to/from JSON files.
    /// </summary>
    [Serializable]
    public class PlayerScores
    {
        public int coins;
    }

    [Serializable]
    public class Scores
    {
        public List<PlayerScores> scores;
    }
}
