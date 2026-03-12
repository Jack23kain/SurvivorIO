using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private float fireRate = 0.8f;
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private int daggerDamage = 10;

    private float fireTimer;

    private void Update()
    {
        if (daggerPrefab == null) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            var target = FindClosestEnemy();
            if (target != null && Vector2.Distance(transform.position, target.transform.position) <= attackRange)
            {
                fireTimer = 0f;
                FireAt(target);
            }
        }
    }

    private EnemyController FindClosestEnemy()
    {
        var enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        EnemyController closest = null;
        float minDist = float.MaxValue;
        foreach (var e in enemies)
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < minDist) { minDist = d; closest = e; }
        }
        return closest;
    }

    private void FireAt(EnemyController target)
    {
        Vector2 dir = target.transform.position - transform.position;
        var go = Instantiate(daggerPrefab, transform.position, Quaternion.identity);
        go.GetComponent<Dagger>()?.Init(dir, damageNumberPrefab, daggerDamage);
    }

    public void UpgradeFireRate() { fireRate = Mathf.Max(0.2f, fireRate * 0.75f); }
    public void UpgradeDamage()   { daggerDamage += 5; }
    public void UpgradeRange()    { attackRange += 2f; }
}
