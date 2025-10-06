using System.Collections.Generic;
using UnityEngine;

public class SwordSwing : MonoBehaviour
{
    public float swingAngle = 90f;
    public Transform pivot;

    private float timer;
    private bool swinging;
    private Quaternion startRot;
    private Quaternion endRot;

    public bool IsSwinging => swinging;

    public PlayerStats stats;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    [SerializeField] private LayerMask obstacleMask;

    private void Awake()
    {
        stats = GetComponentInParent<PlayerStats>();
    }

    private void Update()
    {
        if (!swinging) return;

        timer += Time.deltaTime;
        float t = timer / stats.swingDuration;
        transform.rotation = Quaternion.Slerp(startRot, endRot, t);

        if (t >= 1f)
        {
            swinging = false;
            gameObject.SetActive(false);
            Player.Instance.NotifyAttackFinished();
        }
    }

    public void StartSwing(bool swingLeft, Vector2 direction)
    {
        timer = 0f;
        swinging = true;
        gameObject.SetActive(true);

        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float offset = swingAngle / 2f;
        float fromAngle = baseAngle + (swingLeft ? offset : -offset);
        float toAngle = baseAngle + (swingLeft ? -offset : offset);

        transform.position = pivot.position;
        startRot = Quaternion.Euler(0, 0, fromAngle - 90f);
        endRot = Quaternion.Euler(0, 0, toAngle - 90f);
        transform.rotation = startRot;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null && !hitEnemies.Contains(other.gameObject))
        {
            Vector2 from = pivot.position;
            Vector2 to = enemy.transform.position;

            RaycastHit2D hit = Physics2D.Linecast(from, to, obstacleMask);

            if (hit.collider == null)
            {
                enemy.TakeDamage(stats.baseDamage);
                hitEnemies.Add(other.gameObject);
            }
            else
            {
                Debug.Log($"Blocked by: {hit.collider.name}");
            }
        }
    }

    private void OnDisable()
    {
        hitEnemies.Clear();
    }
}
