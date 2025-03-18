using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
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

    public void Die()
    {
        // เพิ่มคะแนนเมื่อศัตรูถูกทำลาย
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(10);  // เพิ่ม 10 คะแนนสำหรับศัตรูทั่วไป
        }

        // ทำลายศัตรู
        Destroy(gameObject);
    }


}
