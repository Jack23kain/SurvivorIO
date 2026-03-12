using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 20;

    private int currentHp;
    private float invincibilityDuration = 0.5f;
    private float invincibilityTimer;

    private void Start()
    {
        currentHp = maxHp;
    }

    private void Update()
    {
        if (invincibilityTimer > 0f)
            invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        if (invincibilityTimer > 0f) return;
        currentHp -= amount;
        invincibilityTimer = invincibilityDuration;
        Debug.Log($"[Player HP] {currentHp}/{maxHp}");
        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("[Player HP] Player died!");
        // TODO: game over screen
    }
}
