using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;   // Bullet speed
    public int damage = 3;     // Damage dealt by the bullet (set by RangedEnemy)

    private Camera mainCamera;

    void Start()
    {
        // Get the main camera reference
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Check if the bullet is out of bounds of the camera's view
        if (!IsBulletInView())
        {
            Destroy(gameObject); // Destroy the bullet if it's out of bounds
        }
    }

    bool IsBulletInView()
    {
        // Convert the bullet's position to the camera's viewport space
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // Check if the bullet is outside the camera's view (viewpoint is between 0 and 1 on both x and y axes)
        return viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the bullet hits the base tower
        BaseTower2D baseTower = other.GetComponent<BaseTower2D>();
        if (baseTower != null)
        {
            baseTower.TakeDamage(damage);
            Destroy(gameObject);  // Destroy the bullet after it hits the tower
        }
    }
}
