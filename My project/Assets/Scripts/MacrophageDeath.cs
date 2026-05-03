using UnityEngine;
using System.Collections;

public class MacrophageDeath : MonoBehaviour
{
    private Renderer[] renderers;
    private Animator anim;
    private bool hasDied = false;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        anim = GetComponent<Animator>();
    }

    public void TriggerDeath(float delay)
    {
        if (!hasDied)
        {
            StartCoroutine(DeathSequence(delay));
        }
    }

    public void ForceImmediateDeath()
    {
        TriggerDeath(0f);
    }

    IEnumerator DeathSequence(float delay)
    {
        MacrophageFSM fsm = GetComponent<MacrophageFSM>();
        if (fsm != null)
        {
            fsm.isDead = true;
        }
        hasDied = true;

        yield return new WaitForSeconds(delay);

        // 🔥 IMPORTANT FIX: stop animation from overriding transform
        if (anim != null)
        {
            anim.enabled = false;
        }

        float duration = 1.5f;
        float timer = 0f;

        Vector3 startPos = transform.position;

        // 🔥 stronger fall so it's visually obvious
        Vector3 endPos = startPos + new Vector3(
            Random.Range(-0.3f, 0.3f),
            Random.Range(-1.5f, -2.5f),
            Random.Range(-0.3f, 0.3f)
        );

        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(
            Random.Range(60f, 120f),
            Random.Range(0f, 360f),
            Random.Range(-30f, 30f)
        );

        // cache materials (safe URP handling)
        Material[] mats = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                mats[i] = renderers[i].material;
            }
        }

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // 🔥 FORCE WORLD SPACE MOVEMENT
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            Color gray = Color.Lerp(Color.white, Color.gray, t);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null && mats[i] != null)
                {
                    mats[i].color = gray;
                }
            }

            yield return null;
        }

        // final snap to ensure visible end state
        transform.position = endPos;
        transform.rotation = endRot;
    }
}