using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxLife = 100f;
    public float currentLife;

    public float maxStamina = 100f;
    public float currentStamina;

    public int infectionCount = 0;

    public bool isDead = false;

    void Start()
    {
        currentLife = maxLife;
        currentStamina = maxStamina;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("TAKING DAMAGE: " + damage);

        if (isDead) return;

        currentLife -= damage;
        currentLife = Mathf.Clamp(currentLife, 0, maxLife);

        Debug.Log("CURRENT LIFE: " + currentLife);

        if (currentLife <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player died");
        // later: VR death screen / restart logic
    }
}