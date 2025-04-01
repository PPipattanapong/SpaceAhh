using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // เพิ่มการนำเข้า TextMeshPro
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

    public TextMeshProUGUI healthText; // เพิ่มตัวแปร TextMeshProUGUI
    public TextMeshProUGUI dashStatusText; // เพิ่มตัวแปรใหม่เพื่อแสดงสถานะ Dash

    // ตัวแปรที่เกี่ยวข้องกับการ Dash
    public float dashSpeed = 20f;  // ความเร็วของการ Dash
    public float dashDuration = 0.2f;  // ระยะเวลาของ Dash
    public float dashCooldown = 1f;  // เวลาคูลดาวน์ของ Dash

    private float dashTime = 0f;  // เวลาที่ใช้ในการ Dash
    private float dashCooldownTime = 0f;  // เวลาคูลดาวน์ของ Dash

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

        UpdateHealthText(); // อัปเดตข้อความเมื่อเริ่มเกม
    }

    void Update()
    {
        // ตรวจสอบการกดปุ่ม WASD เพื่อเคลื่อนที่
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // หากกด Shift ให้ทำ Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTime <= 0f)
        {
            Dash();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTime > 0f)
        {
            // ถ้าคูลดาวน์ยังไม่หมด ให้แสดงข้อความคูลดาวน์
            UpdateDashStatusText();
        }

        // ตรวจสอบการรีเซ็ตเวลา Dash Cooldown
        if (dashCooldownTime > 0f)
        {
            dashCooldownTime -= Time.deltaTime;
            // อัปเดตข้อความแสดงคูลดาวน์
            UpdateDashStatusText();
        }

        // Handle shooting with 'I' key
        if (Input.GetKeyDown(KeyCode.I)) // การยิงกระสุน
        {
            ShootBullet();
        }
    }

    void Dash()
    {
        // เพิ่มระยะเวลา Dash
        dashTime = dashDuration;

        // รีเซ็ต cooldown
        dashCooldownTime = dashCooldown;

        // เพิ่มการเคลื่อนไหวของ Dash
        Vector2 dashDirection = movement.normalized;  // ทิศทางการ Dash จากการเคลื่อนไหว
        rb.velocity = dashDirection * dashSpeed;  // ตั้งค่าความเร็วของ Rigidbody2D

        // อัปเดตข้อความเมื่อ Dash สำเร็จ
        dashStatusText.text = "Dash ใช้งานได้!";
    }

    void FixedUpdate()
    {
        if (dashTime > 0f)
        {
            // ลดเวลาของ Dash ลง
            dashTime -= Time.fixedDeltaTime;

            // ถ้า Dash เสร็จแล้ว ให้กลับสู่การเคลื่อนที่ปกติ
            if (dashTime <= 0f)
            {
                Vector2 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;


                // ตรวจสอบให้ตำแหน่งของ player ไม่ออกนอกจอ
                Vector3 viewportPosition = Camera.main.WorldToViewportPoint(newPosition);

                // จำกัดตำแหน่งใน viewport ให้อยู่ระหว่าง 0 และ 1
                viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.05f, 0.95f);  // 0.05f และ 0.95f ช่วยให้ player อยู่ใกล้ขอบจอ
                viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.05f, 0.95f);  // จำกัดขอบทั้งบนและล่าง

                // แปลงกลับเป็นตำแหน่งโลก
                newPosition = Camera.main.ViewportToWorldPoint(viewportPosition);


                rb.MovePosition(newPosition);
            }
        }
        else
        {
            // การเคลื่อนไหวปกติ
            Vector2 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;

            // ตรวจสอบให้ตำแหน่งของ player ไม่ออกนอกจอ
            Vector3 viewportPosition = Camera.main.WorldToViewportPoint(newPosition);

            // จำกัดตำแหน่งใน viewport ให้อยู่ระหว่าง 0 และ 1
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.05f, 0.95f);  // 0.05f และ 0.95f ช่วยให้ player อยู่ใกล้ขอบจอ
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.05f, 0.95f);  // จำกัดขอบทั้งบนและล่าง

            // แปลงกลับเป็นตำแหน่งโลก
            newPosition = Camera.main.ViewportToWorldPoint(viewportPosition);

            rb.MovePosition(newPosition);
        }
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

        UpdateHealthText(); // อัปเดตจำนวนเลือดที่เหลือบน TextMeshPro

        if (currentHealth <= 0)
        {
            BaseDestroyed();
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "HP : " + currentHealth.ToString() + " / " + maxHealth.ToString(); // แสดง Health: currentHealth / maxHealth
        }
    }

    void UpdateDashStatusText()
    {
        if (dashStatusText != null)
        {
            if (dashCooldownTime <= 0f)
            {
                dashStatusText.text = "Dash!";
            }
            else
            {
                dashStatusText.text = "Cooldown: " + Mathf.Ceil(dashCooldownTime).ToString() + " Sec";
            }
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

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        // ป้องกันไม่ให้เลือดเกินค่า maxHealth
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // อัปเดตค่า health bar
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        // อัปเดตสีของ health bar
        float healthPercentage = (float)currentHealth / maxHealth;
        if (healthPercentage < 0.2f)
        {
            healthBar.fillRect.GetComponent<Image>().color = lowHealthColor;
        }
        else
        {
            healthBar.fillRect.GetComponent<Image>().color = fullHealthColor;
        }

        // อัปเดตข้อความ Health
        UpdateHealthText();
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            Destroy(collision.gameObject);
        }
    }
}