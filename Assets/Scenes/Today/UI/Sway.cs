using UnityEngine;

public class SwayMovement : MonoBehaviour
{
    public float swayDistance = 0.5f; // ระยะที่ขยับซ้าย-ขวา
    public float swaySpeed = 1f;      // ความเร็วในการแกว่ง

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // จำตำแหน่งเริ่มต้น
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayDistance;
        transform.position = startPos + new Vector3(sway, 0f, 0f); // แกว่งแนว x
    }
}
