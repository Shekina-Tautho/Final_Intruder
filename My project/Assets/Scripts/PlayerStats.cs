using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Life")]
    public float maxLife = 100f;
    public float currentLife;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    public float staminaDrainRate = 10f;   // how fast boost drains
    public float staminaRegenRate = 15f;   // how fast it recovers
    public float staminaRegenDelay = 1.5f; // delay before regen starts

    private float regenTimer = 0f;

    [Header("Other")]
    public int infectionCount = 0;
    public bool isDead = false;

    void Start()
    {
        currentLife = maxLife;
        currentStamina = maxStamina;
    }

    void Update()
    {
         // HandleStaminaRegen(); // disabled for now
    }

    // ---------------- LIFE ----------------
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
    }

    // ---------------- STAMINA ----------------
    public void UseStamina(float amount)
    {
        if (currentStamina <= 0f) return;

        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // reset regen delay every time stamina is used
        regenTimer = staminaRegenDelay;
    }

    public bool HasStamina()
    {
        return currentStamina > 0.1f;
    }

    void HandleStaminaRegen()
    {
        // wait before regenerating
        if (regenTimer > 0f)
        {
            regenTimer -= Time.deltaTime;
            return;
        }

        // regenerate stamina
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    public void AddStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        regenTimer = staminaRegenDelay;
    }
}