using UnityEngine;
using System.Collections;

public class Receptor : MonoBehaviour
{
    public string receptorType;

    private Animator animator;
    private Coroutine animationCoroutine;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    public void PlayMatchAnimation()
    {
        if (animator == null)
        {
            Debug.LogWarning(
                gameObject.name + " does not have an Animator."
            );

            return;
        }

        if (animationCoroutine != null)
        {
            return;
        }

        animator.enabled = true;

        animationCoroutine = StartCoroutine(PlayAnimationTwice());
    }

    private IEnumerator PlayAnimationTwice()
    {
        // Start from the beginning.
        animator.Play("Armature", 0, 0f);

        // Wait for 2 complete animation loops.
        yield return new WaitForSeconds(1.458f * 2f);

        // Stop the animation.
        animator.enabled = false;

        animationCoroutine = null;

        Debug.Log(
            "Receptor animation finished two plays."
        );
    }
}