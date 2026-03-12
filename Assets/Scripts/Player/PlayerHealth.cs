using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 20;
    [SerializeField] private PlayerHPBar hpBar;

    private int currentHp;
    private float invincibilityDuration = 0.5f;
    private float invincibilityTimer;

    private void Start()
    {
        currentHp = maxHp;
        hpBar?.SetRatio(1f);
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
        hpBar?.SetRatio((float)currentHp / maxHp);

        if (currentHp <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        hpBar?.SetRatio((float)currentHp / maxHp);
    }

    private void Die()
    {
        gameObject.SetActive(false); // TODO: replace with game over screen
    }
}
