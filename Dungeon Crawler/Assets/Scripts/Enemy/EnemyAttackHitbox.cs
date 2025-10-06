using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private bool hasHit = false;

    private void OnEnable()
    {
        hasHit = false; // —брос каждый раз, когда активируетс€
        Destroy(gameObject, 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            PlayerStats.Instance.TakeDamage(damage);
            hasHit = true;
        }
    }
}
