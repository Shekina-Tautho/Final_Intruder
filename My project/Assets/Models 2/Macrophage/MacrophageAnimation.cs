
using UnityEngine;

public class MacrophageAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
            return;
        }

        animator.speed = 1f;
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }
}

