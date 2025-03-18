using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BaseTower2D : MonoBehaviour
{
    public GameObject towerBulletPrefab;
    public Transform firePoint1;
    public Transform firePoint2;
    public float bulletSpeed = 5f;

    public int maxHealth = 300;
    private int currentHealth;
    public Slider healthBar;
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Rigidbody2D rb;
    public float moveSpeed = 5f;

    private Vector2 movement; // เพิ่มตัวแปรสำหรับการขยับ

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            healthBar.fillRect.GetComponent<Image>().color = fullHealthColor;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Handle movement - WASD
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Handle shooting with 'I' key
        if (Input.GetKeyDown(KeyCode.I)) // การยิงกระสุน
        {
            ShootBullet();
        }
    }

    void FixedUpdate()
    {
        // Move player
        Vector2 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;

        // Get the player's bounds size
        float playerWidth = spriteRenderer.bounds.extents.x;
        float playerHeight = spriteRenderer.bounds.extents.y;

        // Clamp the position to prevent going outside the screen
        newPosition.x = Mathf.Clamp(newPosition.x, -Camera.main.orthographicSize * Camera.main.aspect + playerWidth, Camera.main.orthographicSize * Camera.main.aspect - playerWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -Camera.main.orthographicSize + playerHeight, Camera.main.orthographicSize - playerHeight);

        rb.MovePosition(newPosition);
    }

    void ShootBullet()
    {
        // การยิงกระสุน
        GameObject bullet1 = Instantiate(towerBulletPrefab, firePoint1.position, Quaternion.identity);
        GameObject bullet2 = Instantiate(towerBulletPrefab, firePoint2.position, Quaternion.identity);

        Rigidbody2D bulletRb1 = bullet1.GetComponent<Rigidbody2D>();
        Rigidbody2D bulletRb2 = bullet2.GetComponent<Rigidbody2D>();

        bulletRb1.velocity = new Vector2(0, bulletSpeed); // ยิงขึ้นด้านบน
        bulletRb2.velocity = new Vector2(0, bulletSpeed); // ยิงขึ้นด้านบนเหมือนกัน
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (healthBar != null)
        {
            healthBar.value = currentHealth;

            float healthPercentage = (float)currentHealth / maxHealth;
            if (healthPercentage < 0.2f)
            {
                healthBar.fillRect.GetComponent<Image>().color = lowHealthColor;
            }
            else
            {
                healthBar.fillRect.GetComponent<Image>().color = fullHealthColor;
            }
        }

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        if (currentHealth <= 0)
        {
            BaseDestroyed();
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    void BaseDestroyed()
    {
        // เริ่มต้นการโหลด scene หลังจากทำลาย
        StartCoroutine(LoadLoseScene());
    }

    IEnumerator LoadLoseScene()
    {
        // รอ 1 เฟรมก่อนการโหลด scene
        yield return null;

        // โหลด scene "Lose"
        SceneManager.LoadScene("Lose");

        // ทำลาย Tower หลังจากโหลด scene
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
}
