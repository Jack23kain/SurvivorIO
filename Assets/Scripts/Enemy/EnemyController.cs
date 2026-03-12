using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int maxHp = 9;
    [SerializeField] private float despawnDistance = 25f;

    private int currentHp;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
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
        sr = GetComponent<SpriteRenderer>();
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

    private void Update()
    {
        if (player == null) return;
        if (Vector2.Distance(rb.position, player.position) > despawnDistance)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (player == null) return;
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        if (dir.x > 0.01f)       sr.flipX = false;
        else if (dir.x < -0.01f) sr.flipX = true;
    }

    // Enemy collider is a trigger — detect daggers and player here
    private void OnTriggerEnter2D(Collider2D other)
    {
        var dagger = other.GetComponent<Dagger>();
        if (dagger != null)
        {
            TakeDamage(dagger.Damage);
            dagger.HitEnemy(transform.position);
            return;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        other.GetComponent<PlayerHealth>()?.TakeDamage(1);
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
