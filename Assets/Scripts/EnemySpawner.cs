using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefabA;        // ศัตรูประเภท A
    public GameObject enemyPrefabB;        // ศัตรูประเภท B
    public float initialSpawnInterval = 5f; // ค่าเริ่มต้นของ spawn interval
    public float minSpawnInterval = 1f;    // ค่าต่ำสุดของ spawn interval
    public float spawnIntervalDecreaseRate = 0.05f; // ความเร็วที่ spawn interval ลดลง
    public int maxEnemies = 3;            // จำนวนศัตรูสูงสุดที่สามารถ spawn ในครั้งเดียว
    public float difficultyIncreaseTime = 30f; // เวลาในการเพิ่มความยาก (ทุก 30 วินาที)

    private float nextSpawnTime;
    private float currentSpawnInterval;
    private int currentEnemyCount = 1;    // เริ่มต้นที่ 1 ตัวศัตรู
    private float timeSinceLastDifficultyIncrease = 0f; // เวลาผ่านไปนับจากการเพิ่มความยากครั้งล่าสุด

    void Start()
    {
        // ตั้งค่า spawn interval เริ่มต้น
        currentSpawnInterval = initialSpawnInterval;
        nextSpawnTime = Time.time + currentSpawnInterval;
    }

    void Update()
    {
        // เพิ่มเวลานับตั้งแต่การเพิ่มความยากครั้งล่าสุด
        timeSinceLastDifficultyIncrease += Time.deltaTime;

        // ทุกๆ 30 วินาที ให้เพิ่มความยาก
        if (timeSinceLastDifficultyIncrease >= difficultyIncreaseTime)
        {
            IncreaseDifficulty();
            timeSinceLastDifficultyIncrease = 0f; // รีเซ็ตเวลา
        }

        // เช็คเวลาที่จะ spawn ศัตรู
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemies();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }

    void SpawnEnemies()
    {
        // สุ่มประเภทศัตรู (ประเภท A หรือ B)
        for (int i = 0; i < currentEnemyCount; i++)
        {
            // สุ่มศัตรูประเภท A หรือ B
            GameObject enemyPrefab = (Random.value > 0.5f) ? enemyPrefabA : enemyPrefabB;

            // สุ่มตำแหน่งในแกน X
            float spawnX = Random.Range(-10f, 10f);
            Vector2 spawnPosition = new Vector2(spawnX, Camera.main.orthographicSize + 1); // เริ่มต้นจากด้านบน

            // สร้างศัตรู
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    void IncreaseDifficulty()
    {
        // ลด spawn interval ลง (ให้การ spawn ศัตรูบ่อยขึ้น)
        currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - spawnIntervalDecreaseRate);

        // เพิ่มจำนวนศัตรูในแต่ละครั้ง
        currentEnemyCount = Mathf.Min(maxEnemies, currentEnemyCount + 1); // เพิ่มขึ้นทีละน้อย
    }
}
