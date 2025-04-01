using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public static float timeElapsed; // เปลี่ยนเป็น static เพื่อให้สามารถเข้าถึงจากสคริปต์อื่นๆ ได้

    void Start()
    {
        timeElapsed = 0f; // เริ่มต้นเวลา
    }

    void Update()
    {
        timeElapsed += Time.deltaTime; // เพิ่มเวลาที่ผ่านไปในแต่ละเฟรม
        DisplayTime(timeElapsed); // อัพเดทข้อความที่จะแสดงเวลา
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); // แปลงเป็นนาที
        float seconds = Mathf.FloorToInt(timeToDisplay % 60); // แปลงเป็นวินาที

        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }
}
