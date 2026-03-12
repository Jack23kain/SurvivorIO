using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Dagger : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;

    private Rigidbody2D rb;
    private Vector2 direction;

    public void Init(Vector2 dir)
    {
        rb = GetComponent<Rigidbody2D>();
        direction = dir.normalized;
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
            enemy.TakeDamage(999);
            Destroy(gameObject);
        }
    }
}
