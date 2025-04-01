using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public GameObject item1Prefab;  // Prefab ของไอเทมที่ 1
    public GameObject item2Prefab;  // Prefab ของไอเทมที่ 2

    public float item1DropChance = 0.4f; // โอกาสที่จะดรอปไอเทมที่ 1 (40%)
    public float item2DropChance = 0.2f; // โอกาสที่จะดรอปไอเทมที่ 2 (20%)

    public float speed = 2f;            // Movement speed of the enemy
    public float dashSpeed = 10f;       // Speed during the dash attack
    public int damage = 5;              // Damage to the tower (normal attack)
    public int dashDamage = 15;         // Damage during dash attack
    public float attackInterval = 1f;   // Time between each attack (prevents rapid attacks)

    private float nextAttackTime;       // Timer to control attack frequency
    private Transform target;           // Reference to the tower's transform
    private BaseTower2D baseTower;      // Reference to the BaseTower2D component
    private Rigidbody2D rb;             // Reference to the Rigidbody2D component

    private enum EnemyState { Moving, StepBack, Charging, Attacking }
    private EnemyState currentState = EnemyState.Moving;

    private float stepBackDuration = 1f;  // Duration of the step back
    private float chargeDuration = 1f;    // Duration of the charge
    private float stepBackTime = 0f;      // Timer for step back
    private float chargeTime = 0f;        // Timer for charge

    // Enemy health
    public int maxHealth = 15;           // Max health of the enemy
    private int currentHealth;           // Current health of the enemy

    private SpriteRenderer spriteRenderer;  // Reference to the sprite renderer to change color

    private bool isDead = false; // ตัวแปรเพื่อเช็คว่า Enemy ตายแล้วหรือยัง

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Find the tower by its tag and get its BaseTower2D component
        GameObject towerObject = GameObject.FindGameObjectWithTag("Tower");
        if (towerObject != null)
        {
            baseTower = towerObject.GetComponent<BaseTower2D>();
            target = towerObject.transform;
        }

        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component for movement
        spriteRenderer = GetComponent<SpriteRenderer>();  // Get the SpriteRenderer component
    }

    void Update()
    {
        // Only execute movement behavior if the target is found
        if (target != null)
        {
            switch (currentState)
            {
                case EnemyState.Moving:
                    MoveTowardsTower();
                    break;
                case EnemyState.StepBack:
                    StepBack();
                    break;
                case EnemyState.Charging:
                    Charge();
                    break;
                case EnemyState.Attacking:
                    Attack();
                    break;
            }
        }
    }

    void MoveTowardsTower()
    {
        // Move the enemy towards the tower
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;  // Use Rigidbody2D velocity for movement

        // Transition to StepBack state when within a certain distance
        if (Vector2.Distance(transform.position, target.position) <= 5f) // Set desired range to start step back
        {
            currentState = EnemyState.StepBack;
            stepBackTime = Time.time; // Start the step-back timer
        }
    }

    void StepBack()
    {
        // Move the enemy backward for a brief time
        Vector2 direction = (transform.position - target.position).normalized;  // Move away from tower
        rb.velocity = direction * speed;

        // After stepping back for the defined duration, start charging
        if (Time.time - stepBackTime >= stepBackDuration)
        {
            currentState = EnemyState.Charging;
            chargeTime = Time.time;  // Start charging timer
        }
    }

    void Charge()
    {
        // Increase speed for charging the tower
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * dashSpeed;

        // After charging for the defined duration, attack the tower
        if (Time.time - chargeTime >= chargeDuration)
        {
            currentState = EnemyState.Attacking;
        }
    }

    void Attack()
    {
        // Apply dash damage to the tower when the enemy touches it
        if (baseTower != null)
        {
            baseTower.TakeDamage(dashDamage);
        }

        // After attacking, destroy the enemy (it disappears)
        Die();
    }

    // This method is called when the enemy enters a trigger collider (for when attacking the tower)
    void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the enemy collides with the tower (identified by the "Tower" tag)
        if (collider.gameObject.CompareTag("Tower") && currentState == EnemyState.Attacking)
        {
            // Apply dash damage to the tower when the enemy dashes into it
            if (baseTower != null)
            {
                baseTower.TakeDamage(dashDamage);
            }

            // Destroy the enemy after dash attack
            Die();
        }
    }

    // Method to handle damage from TowerBullet
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return; // ถ้า Enemy ตายแล้วไม่ให้รับความเสียหายอีก

        currentHealth -= damageAmount;  // Subtract the damage

        // Change color to red when damaged and then back to the original color
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed()); // Start the flash effect
        }

        // Check if enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        // Save the original color
        Color originalColor = spriteRenderer.color;

        // Change to red
        spriteRenderer.color = Color.red;

        // Wait for a short time (flash duration)
        yield return new WaitForSeconds(0.1f);

        // Restore the original color
        spriteRenderer.color = originalColor;
    }

    // เพิ่มการดรอปไอเทมเมื่อศัตรูตาย
    public void Die()
    {
        if (isDead) return; // ถ้า Enemy ตายแล้วไม่ให้ทำลายซ้ำ

        // Mark as dead
        isDead = true;

        // เพิ่มคะแนนเมื่อศัตรูถูกทำลาย
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(10);  // เพิ่ม 10 คะแนนสำหรับศัตรูทั่วไป
            Debug.Log("Enemy destroyed. Score added: 10");  // เพิ่มบรรทัดนี้เพื่อการตรวจสอบ
        }

        // Randomly drop items
        DropItems();

        // ทำลายศัตรู
        Destroy(gameObject);
    }

    // ฟังก์ชันสำหรับการดรอปไอเทม
    void DropItems()
    {
        float dropChance = Random.value; // Generate a random value between 0 and 1

        // ตรวจสอบว่าโอกาสสุ่มดรอปไอเทมที่ 1 หรือ 2
        if (dropChance <= item1DropChance)
        {
            Instantiate(item1Prefab, transform.position, Quaternion.identity); // ดรอปไอเทมที่ 1
            Debug.Log("Item 1 dropped!");
        }
        else if (dropChance <= item1DropChance + item2DropChance)
        {
            Instantiate(item2Prefab, transform.position, Quaternion.identity); // ดรอปไอเทมที่ 2
            Debug.Log("Item 2 dropped!");
        }
    }
}
