using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;  // Instance สำหรับ Singleton pattern
    public static int score = 0;  // Static variable to hold the score
    public TextMeshProUGUI scoreText;  // Reference to the TextMeshPro UI text

    void Awake()
    {
        // ตรวจสอบว่า Instance มีอยู่แล้วหรือไม่
        if (Instance == null)
        {
            Instance = this;  // กำหนด Instance เป็นตัวเอง
        }
        else
        {
            Destroy(gameObject);  // หากมี Instance อยู่แล้วให้ลบตัวเอง
        }
    }

    void Start()
    {
        UpdateScoreDisplay();  // Initialize the score display at the start
    }

    // Method ที่เพิ่มคะแนน
    public void AddScore(int points)
    {
        score += points;  // เพิ่มคะแนน
        UpdateScoreDisplay();  // อัปเดตการแสดงผลคะแนน
    }

    // Method ที่อัปเดตการแสดงผลคะแนน
    void UpdateScoreDisplay()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    // Method รีเซตคะแนน
    public void ResetScore()
    {
        score = 0;  // รีเซตคะแนนเป็น 0
        UpdateScoreDisplay();  // อัปเดตการแสดงผลคะแนน
    }
}
