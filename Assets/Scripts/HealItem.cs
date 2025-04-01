using UnityEngine;

public class HealItem : MonoBehaviour
{
    public int healAmount = 20; // จำนวนเลือดที่ฟื้นฟู

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าอ็อบเจ็กต์ที่ชนมี tag เป็น "Tower"
        if (other.CompareTag("Tower"))
        {
            // ดึงคอมโพเนนต์ BaseTower2D จาก Tower
            BaseTower2D tower = other.GetComponent<BaseTower2D>();

            if (tower != null)
            {
                // ฟื้นฟูเลือดให้กับ Tower
                tower.Heal(healAmount);

                // ลบไอเท็มหลังจากเก็บแล้ว
                Destroy(gameObject);
            }
        }
    }
}
