using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;  // Static variable to hold the score
    public TextMeshProUGUI scoreText;  // Reference to the TextMeshPro UI text

    void Start()
    {
        UpdateScoreDisplay();  // Initialize the score display at the start
    }

    // This method is called to increment the score when an enemy is destroyed
    public void AddScore(int points)
    {
        score += points;  // Add points to the score
        UpdateScoreDisplay();  // Update the score display
    }

    // Update the score display on the screen
    void UpdateScoreDisplay()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
