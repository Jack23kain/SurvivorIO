using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int maxHp = 3;

    private int currentHp;
    private Rigidbody2D rb;
    private Transform player;

    public void Init(Transform playerTransform, float speed)
    {
        player = playerTransform;
        currentHp = maxHp;
        moveSpeed = speed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHp = maxHp;
        if (player == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
