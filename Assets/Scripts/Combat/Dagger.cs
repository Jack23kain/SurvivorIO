using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Dagger : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject damageNumberPrefab;

    public int Damage => damage;

    private Rigidbody2D rb;
    private Vector2 direction;

    public void Init(Vector2 dir, GameObject dmgNumberPrefab, int damageOverride = -1)
    {
        rb = GetComponent<Rigidbody2D>();
        direction = dir.normalized;
        damageNumberPrefab = dmgNumberPrefab;
        if (damageOverride >= 0) damage = damageOverride;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Destroy(gameObject, lifetime);
    }

    // Called by EnemyController when the dagger hits
    public void HitEnemy(Vector3 spawnPos)
    {
        if (damageNumberPrefab != null)
            Instantiate(damageNumberPrefab, spawnPos, Quaternion.identity)
                .GetComponent<DamageNumber>()?.Init(damage);

        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (rb != null)
            rb.linearVelocity = direction * speed;
    }
}
