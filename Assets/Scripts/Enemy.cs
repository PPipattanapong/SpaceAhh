using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    [Header("Item Drop")]
    public GameObject item1Prefab;
    public GameObject item2Prefab;
    [Range(0f, 1f)] public float item1DropChance = 0.4f;
    [Range(0f, 1f)] public float item2DropChance = 0.2f;

    [Header("Movement and Attack")]
    public float speed = 2f;
    public float dashSpeed = 10f;
    public int damage = 5;
    public int dashDamage = 15;
    public float attackInterval = 1f;

    [Header("State Durations")]
    public float stepBackDuration = 1f;
    public float chargeDuration = 1f;

    [Header("Health")]
    public int maxHealth = 15;

    private int currentHealth;
    private bool isDead = false;

    private Transform target;
    private BaseTower2D baseTower;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float nextAttackTime;
    private float stepBackTime;
    private float chargeTime;

    private enum EnemyState { Moving, StepBack, Charging, Attacking }
    private EnemyState currentState = EnemyState.Moving;

    void Start()
    {
        currentHealth = maxHealth;

        GameObject tower = GameObject.FindGameObjectWithTag("Tower");
        if (tower != null)
        {
            baseTower = tower.GetComponent<BaseTower2D>();
            target = tower.transform;
        }

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead || target == null) return;

        switch (currentState)
        {
            case EnemyState.Moving: MoveTowardsTower(); break;
            case EnemyState.StepBack: StepBack(); break;
            case EnemyState.Charging: Charge(); break;
            case EnemyState.Attacking: Attack(); break;
        }
    }

    void MoveTowardsTower()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;

        if (Vector2.Distance(transform.position, target.position) <= 5f)
        {
            currentState = EnemyState.StepBack;
            stepBackTime = Time.time;
        }
    }

    void StepBack()
    {
        Vector2 direction = (transform.position - target.position).normalized;
        rb.velocity = direction * speed;

        if (Time.time - stepBackTime >= stepBackDuration)
        {
            currentState = EnemyState.Charging;
            chargeTime = Time.time;
        }
    }

    void Charge()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * dashSpeed;

        if (Time.time - chargeTime >= chargeDuration)
        {
            currentState = EnemyState.Attacking;
        }
    }

    void Attack()
    {
        if (baseTower != null)
        {
            baseTower.TakeDamage(dashDamage);
        }

        Die();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Tower") && currentState == EnemyState.Attacking)
        {
            if (baseTower != null)
            {
                baseTower.TakeDamage(dashDamage);
            }

            Die();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (spriteRenderer != null)
            StartCoroutine(FlashRed());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator FlashRed()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = original;
    }

    void DropItems()
    {
        float dropChance = Random.value;

        if (dropChance <= item1DropChance)
        {
            Instantiate(item1Prefab, transform.position, Quaternion.identity);
            Debug.Log("Item 1 dropped!");
        }
        else if (dropChance <= item1DropChance + item2DropChance)
        {
            Instantiate(item2Prefab, transform.position, Quaternion.identity);
            Debug.Log("Item 2 dropped!");
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(10);
            Debug.Log("Enemy destroyed. Score added: 10");
        }

        DropItems();

        DestroyWithAnimation destroyer = GetComponent<DestroyWithAnimation>();
        if (destroyer != null)
        {
            destroyer.DestroyObject();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
