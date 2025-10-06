using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    
    [Header("Base Stats")]
    public int maxHealth = 5;
    public float moveSpeed = 5f;
    public float attackCooldown = 0.5f;
    public float moveSpeedDuringAttack = 2.5f;
    public float swingDuration = 0.2f;
    public int baseDamage = 10;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.6f;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1f;
    public float flashInterval = 0.1f;

    [SerializeField] private SpriteRenderer[] spriteParts;

    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    [Header("References")]
    [SerializeField] private PlayerHealthUI healthUI;

    [ContextMenu("Добавить контейнер здоровья")]

    private void Editor_AddContainer()
        {
            AddHealthContainer();
        }

    [ContextMenu("Убрать контейнер здоровья")]
    private void Editor_RemoveContainer()
        {
            RemoveHealthContainer();
        }

    [ContextMenu("Получить урон")]
    private void Editor_TakeDamage()
        {
            TakeDamage(1);
        }

    [ContextMenu("Лечение")]
    private void Editor_Heal()
        {
            Heal(1);
        }

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthUI.Init(maxHealth);
        healthUI.SetHealth(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        healthUI.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invincibilityDuration)
        {
            visible = !visible;

            foreach (var sr in spriteParts)
            {
                if (sr != null)
                    sr.enabled = visible;
            }

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        foreach (var sr in spriteParts)
        {
            if (sr != null)
                sr.enabled = true;
        }

        isInvincible = false;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthUI.SetHealth(currentHealth);
    }

    public void AddHealthContainer()
    {
        maxHealth++;
        currentHealth++;
        healthUI.AddContainer();
        healthUI.SetHealth(currentHealth);
    }

    public void RemoveHealthContainer()
    {
        if (maxHealth > 1)
        {
            maxHealth--;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            healthUI.RemoveContainer();
            healthUI.SetHealth(currentHealth);
        }
    }

    private void Die()
    {
        Debug.Log("Player died.");
    }
}
