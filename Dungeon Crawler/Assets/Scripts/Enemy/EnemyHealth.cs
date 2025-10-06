using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    private Animator animator;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    public UnityEvent OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        Debug.Log($"{gameObject.name} took {amount} damage. Current HP: {currentHealth} / {maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Die() called");

        if (TryGetComponent(out UnityEngine.AI.NavMeshAgent agent)) agent.enabled = false;
        if (TryGetComponent(out Collider2D col)) col.enabled = false;
        if (TryGetComponent(out EnemyAI ai)) ai.enabled = false;

        OnDeath?.Invoke();

        if (animator != null)
        {
            animator.SetTrigger("Die");
            animator.SetBool("IsMoving", false);
        }
        else
        {
            Debug.LogWarning("Animator not found on enemy!");
        }

        Destroy(gameObject, 2f);
    }
}
