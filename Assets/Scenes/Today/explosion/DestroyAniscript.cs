using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DestroyWithAnimation : MonoBehaviour
{
    private Animator animator;
    private bool isDestroying = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void DestroyObject()
    {
        if (isDestroying) return;
        isDestroying = true;

        animator.SetTrigger("Explode");

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        // Destroy after the animation finishes
        float animDuration = GetAnimationLength("Explode"); // ใช้ชื่อ animation ที่ตั้งไว้ใน Animator
        Destroy(gameObject, animDuration > 0 ? animDuration : 1f); // fallback ถ้าไม่เจอความยาว
    }

    public void OnAnimationEnd() => Destroy(gameObject);

    private float GetAnimationLength(string clipName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 1f;
    }
}
