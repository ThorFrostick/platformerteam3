using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    //The JSON file we will be reading and writing leaderboard data to/from.
    [SerializeField]
    private TextAsset json_file;

    //The display text that holds the information from the JSON file.
    private string leaderboard_data;

    //The TextMeshPro object we will use to display the data from the JSON file.
    [SerializeField]
    private TextMeshProUGUI text_display;


    private void Start()
    {
        LoadLeaderboard();
    }

    /// <summary>
    /// Add the player's score to the JSON leaderboard.
    /// </summary>
    private void AddScore()
    {

    }
    
    /// <summary>
    /// Load data from a JSON object and display it in a text field.
    /// </summary>
    private void LoadLeaderboard()
    {
        //Initialize our leaderboard text.
        leaderboard_data = "Scores:\n";
        
        //Load our data from the JSON file, and pass it into our display string.
        leaderboard_data += json_file.text;

        //Display the leaderboard data on our Text object.
        text_display.text = leaderboard_data;
    }
}
