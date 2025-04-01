using UnityEngine;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    public TextMeshProUGUI timeText; // TextMesh Pro UI ที่แสดงเวลา
    public TextMeshProUGUI scoreText; // TextMesh Pro UI ที่แสดงคะแนน

    void Start()
    {
        // ดึงข้อมูลเวลาและคะแนนจาก Timer และ ScoreManager
        string timeFormatted = FormatTime(Timer.timeElapsed);
        timeText.text = "Time : " + timeFormatted;

        scoreText.text = "Score : " + ScoreManager.score.ToString();

        // รีเซตคะแนนหลังแสดงผลลัพธ์
        ScoreManager.Instance.ResetScore();
    }

    // ฟังก์ชั่นที่แปลงเวลาเป็นรูปแบบที่ต้องการ
    string FormatTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}