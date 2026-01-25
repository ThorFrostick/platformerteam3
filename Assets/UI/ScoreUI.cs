using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateUI;
        UpdateUI(ScoreManager.Instance.Score);
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= UpdateUI;
    }

    private void UpdateUI(int newScore)
    {
        Debug.Log($"UI Update Score: {newScore} ");
        scoreText.text = $"Score: {newScore}";
    }
}
