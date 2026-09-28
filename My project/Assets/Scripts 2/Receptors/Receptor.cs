using UnityEngine;

public class Receptor : MonoBehaviour
{
    public string receptorType;

    private Animator animator;

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

        // Turn the Animator on.
        animator.enabled = true;

        // Play the Armature animation from the beginning.
        animator.Play("Armature", 0, 0f);

        Debug.Log(
            "Playing receptor animation on " +
            gameObject.name
        );
    }
}