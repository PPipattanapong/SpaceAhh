using UnityEngine;
using System.Collections;

public class RangedEnemy : MonoBehaviour
{
    public GameObject bulletPrefab;      // Bullet prefab to shoot
    public float shootingRange = 3f;     // ลดระยะการยิงให้ใกล้ขึ้น
    public float speed = 3f;             // เพิ่มความเร็วของศัตรู
    public float fireRate = 2f;          // ความถี่ที่ศัตรูยิง (2 วินาที)

    public int maxHealth = 10;           // Max health for the enemy
    private int currentHealth;           // Current health of the enemy

    private GameObject target;           // Automatically find the target (tower) at runtime
    private float nextFireTime;
    private Transform firePoint;         // Reference to the fire point (child object)
    private SpriteRenderer spriteRenderer;  // Reference to the sprite renderer to change color
    private Color originalColor;         // Store the original color of the enemy

    private BaseTower2D baseTower;       // Reference to the BaseTower2D component
    private BaseTower2D player;               // Reference to the player for damage

    void Start()
    {
        // Set the enemy's health to maxHealth at the start
        currentHealth = maxHealth;

        // Automatically find the fire point by name (assuming it exists as a child of this GameObject)
        firePoint = transform.Find("FirePoint");

        if (firePoint == null)
        {
            Debug.LogError("FirePoint not found! Make sure you have a child object named 'FirePoint' under the RangedEnemy prefab.");
        }

        // Find the tower by its tag (you can set the tag on the tower in the Unity Editor)
        target = GameObject.FindGameObjectWithTag("Tower");

        // Ensure there's a target assigned, otherwise log an error
        if (target == null)
        {
            Debug.LogError("Target not found! Ensure the tower GameObject has the 'Tower' tag assigned.");
        }

        // Get the BaseTower2D component for enemy count increment
        GameObject towerObject = GameObject.FindGameObjectWithTag("Tower");
        if (towerObject != null)
        {
            baseTower = towerObject.GetComponent<BaseTower2D>();
        }

        // Get the Player component
        player = GameObject.FindGameObjectWithTag("Tower").GetComponent<BaseTower2D>();

        nextFireTime = Time.time + fireRate;
        spriteRenderer = GetComponent<SpriteRenderer>();  // Get the SpriteRenderer component
        originalColor = spriteRenderer.color;             // Store the original color
    }

    void Update()
    {
        // Move towards the target (tower) continuously, even within the shooting range
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }

        // Shoot if within range and it's time to fire
        if (Time.time >= nextFireTime && Vector2.Distance(transform.position, target.transform.position) <= shootingRange)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null && target != null)
        {
            // Instantiate the bullet at the fire point's position and rotation
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Get Bullet script and set target direction
            Bullet bulletScript = bullet.GetComponent<Bullet>();

            // ยิงกระสุนลงด้านล่าง
            Vector2 direction = Vector2.down; // ทิศทางกระสุนลงด้านล่าง

            // Disable gravity on the Rigidbody2D
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;  // Ensure gravity is disabled

            // Set bullet velocity towards the target
            rb.velocity = direction * bulletScript.speed;
        }
    }

    // Function to handle damage from bullets
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;  // Subtract the damage

        // Change color to red when damaged
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

        // Start coroutine to reset the color after a short delay
        if (spriteRenderer != null)
        {
            StartCoroutine(ResetColor());
        }

        // Check if the RangedEnemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Coroutine to reset the sprite color to original after a short flash of red
    IEnumerator ResetColor()
    {
        // Wait for a brief moment and reset the color
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;  // Reset to the original color
        }
    }

    // Function to destroy the enemy when its health reaches 0
    public void Die()
    {
        // เพิ่มคะแนนเมื่อศัตรูถูกทำลาย
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(20);  // เพิ่ม 20 คะแนนสำหรับ RangedEnemy
        }

        // ทำลายศัตรู
        Destroy(gameObject);
    }

    // Detect collision with the bullet
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TowerBullet"))  // Ensure this is the bullet
        {
            TowerBullet bullet = other.GetComponent<TowerBullet>();
            if (bullet != null)
            {
                // Take damage from the bullet
                TakeDamage(bullet.damage);

                // Destroy the bullet after it hits
                Destroy(other.gameObject);
            }
        }

        // Detect collision with the player
        if (other.CompareTag("Tower"))
        {
            // ทำความเสียหาย 15 ให้ผู้เล่น
            player.TakeDamage(15);

            // ทำลายตัวเอง
            Destroy(gameObject);
        }
    }
}
