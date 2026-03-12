using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Dagger : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject damageNumberPrefab;

    private Rigidbody2D rb;
    private Vector2 direction;

    public void Init(Vector2 dir, GameObject dmgNumberPrefab)
    {
        rb = GetComponent<Rigidbody2D>();
        direction = dir.normalized;
        damageNumberPrefab = dmgNumberPrefab;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (rb != null)
            rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            if (damageNumberPrefab != null)
                Instantiate(damageNumberPrefab, enemy.transform.position, Quaternion.identity)
                    .GetComponent<DamageNumber>()?.Init(damage);

            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
