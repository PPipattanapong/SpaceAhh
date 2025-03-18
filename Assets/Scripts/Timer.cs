using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // TextMesh Pro UI ที่จะใช้แสดงเวลา
    private float timeElapsed; // ตัวแปรเก็บเวลาที่ผ่านไป

    void Start()
    {
        timeElapsed = 0f; // เริ่มต้นเวลา
    }

    void Update()
    {
        timeElapsed += Time.deltaTime; // เพิ่มเวลาที่ผ่านไปในแต่ละเฟรม
        DisplayTime(timeElapsed); // อัพเดทข้อความที่จะแสดงเวลา
    }

    // ฟังก์ชั่นที่แปลงเวลาเป็นรูปแบบที่ต้องการ
    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); // แปลงเป็นนาที
        float seconds = Mathf.FloorToInt(timeToDisplay % 60); // แปลงเป็นวินาที

        // แสดงเวลาในรูปแบบ "Time: MM:SS"
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }
}
