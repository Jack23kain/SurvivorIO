using UnityEngine;

public class XPGem : MonoBehaviour
{
    [SerializeField] private int xpValue = 1;
    [SerializeField] private float pickupRadius = 2.5f;
    [SerializeField] private float pullSpeed = 10f;

    private Transform player;
    private XPManager xpManager;

    private void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            xpManager = p.GetComponent<XPManager>();
        }
    }

    private void Update()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > pickupRadius) return;

        transform.position = Vector3.MoveTowards(
            transform.position, player.position, pullSpeed * Time.deltaTime);

        if (dist < 0.15f)
        {
            xpManager?.AddXP(xpValue);
            Destroy(gameObject);
        }
    }
}
