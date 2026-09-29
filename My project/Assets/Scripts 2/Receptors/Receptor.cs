using UnityEngine;
using System.Collections;

public class Receptor : MonoBehaviour
{
    public string receptorType;

    [Header("Animation")]
    public AnimationClip matchAnimation;

    private Animator animator;
    private Coroutine animationCoroutine;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            // Prevent the animation from playing automatically.
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

        if (matchAnimation == null)
        {
            Debug.LogWarning(
                gameObject.name + " has no Match Animation assigned."
            );

            return;
        }

        // Prevent the animation from being triggered repeatedly
        // while it is already playing.
        if (animationCoroutine != null)
        {
            return;
        }

        animator.enabled = true;

        animationCoroutine = StartCoroutine(PlayAnimationTwice());

        Debug.Log(
            "Playing receptor animation twice on " +
            gameObject.name
        );
    }

    private IEnumerator PlayAnimationTwice()
    {
        float animationLength = matchAnimation.length;

        // FIRST PLAY
        animator.Play("Armature", 0, 0f);

        yield return new WaitForSeconds(animationLength);

        // SECOND PLAY
        animator.Play("Armature", 0, 0f);

        yield return new WaitForSeconds(animationLength);

        // Finished both plays
        animator.enabled = false;

        animationCoroutine = null;
    }
}