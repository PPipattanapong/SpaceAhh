using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    public float speed = 5f;   // Bullet speed
    public int damage = 5;     // Bullet damage

    void Update()
    {
        // Move the bullet upwards
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the bullet hits an enemy (identified by the "Enemy" tag)
        if (other.CompareTag("Enemy"))
        {
            // Deal damage to the enemy
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Destroy the bullet after it hits the enemy
            Destroy(gameObject);
        }
    }

    // Destroy the bullet when it goes off-screen
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
