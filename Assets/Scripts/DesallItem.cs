using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public int pointsPerEnemy = 30;  // กำหนดคะแนนที่เพิ่มเมื่อทำลายศัตรูแต่ละตัว

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            // ทำลายศัตรูทั้งหมดและเพิ่มคะแนน
            DestroyEnemies();

            // ทำลาย item หลังจากเก็บ
            Destroy(gameObject);
        }
    }

    void DestroyEnemies()
    {
        // หาศัตรูทั้งหมดในฉากที่มี tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // ทำลายศัตรูทุกตัว
        foreach (GameObject enemy in enemies)
        {
            // เพิ่มคะแนนสำหรับการทำลายศัตรูแต่ละตัว
            ScoreManager.Instance.AddScore(pointsPerEnemy);

            // ทำลายศัตรู
            Destroy(enemy);
        }
    }
}
