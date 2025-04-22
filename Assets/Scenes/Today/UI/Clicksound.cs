using UnityEngine;

public class PlayClickSound : MonoBehaviour
{
    public AudioClip clickSound;
    private static AudioSource audioSource;

    void Awake()
    {
        // ใช้ AudioSource เดียว แชร์ทั้งเกม
        if (audioSource == null)
        {
            GameObject audioObj = new GameObject("ButtonSoundPlayer");
            DontDestroyOnLoad(audioObj); // ไม่ให้หายตอนโหลดฉาก
            audioSource = audioObj.AddComponent<AudioSource>();
        }
    }

    public void PlaySound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
