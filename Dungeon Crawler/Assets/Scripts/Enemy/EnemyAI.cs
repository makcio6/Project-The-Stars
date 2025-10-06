using UnityEngine;
using UnityEngine.AI;
using Crawler.Utils;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private State startingState;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.0f;

    [ContextMenu("Test Attack")]
    private void DebugTestAttack()
    {
        Debug.Log("Called DebugTestAttack()");
        TryAttack();
    }

    public GameObject attackHitbox;

    private float idleTimer = 0f;
    private float idleTimeMax;
    private float lastAttackTime;

    private Enemy1Visual enemyVisual;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private State state;
    private float roamingTime;
    private Vector3 roamPosition;
    private Vector3 startingPosition;

    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public float detectionRange = 8f;
    public LayerMask obstacleMask;

    private Transform player;
    private Vector3 lastKnownPlayerPosition;

    public bool isAttacking;
    public bool IsAttacking() => isAttacking;

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Investigating
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyVisual = GetComponentInChildren<Enemy1Visual>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        state = startingState;
    }


    private void Start()
    {
        startingPosition = transform.position;
        player = Player.Instance.transform;
    }

    private void Update()
    {
        //Debug.Log("Current state: " + state);

        if (!isAttacking)
        {
            switch (state)
            {
                case State.Idle:
                    //Debug.Log("IDLE: timer = " + idleTimer);
                    idleTimer -= Time.deltaTime;
                    if (idleTimer <= 0)
                    {
                        Roaming();
                        roamingTime = roamingTimerMax;
                        state = State.Roaming;
                    }
                    DetectPlayer();
                    break;

                case State.Roaming:
                    //Debug.Log("ROAMING: time = " + roamingTime);
                    roamingTime -= Time.deltaTime;
                    if (roamingTime < 0)
                    {
                        state = State.Idle;
                        idleTimeMax = Random.Range(1f, 5f);
                        idleTimer = idleTimeMax;
                        navMeshAgent.ResetPath();
                    }
                    DetectPlayer();
                    break;

                case State.Chasing:
                    //Debug.Log("CHASING: distance to player = " + Vector2.Distance(transform.position, player.position));
                    if (CanSeePlayer())
                    {
                        lastKnownPlayerPosition = player.position;

                        float distToPlayer = Vector2.Distance(transform.position, player.position);

                        if (!isAttacking)
                        {
                            if (distToPlayer > attackRange)
                            {
                                navMeshAgent.SetDestination(player.position);
                            }
                            else if (Time.time >= lastAttackTime + attackCooldown)
                            {
                                TryAttack();
                            }
                        }
                    }
                    else
                    {
                        state = State.Investigating;
                        navMeshAgent.isStopped = false;
                        navMeshAgent.SetDestination(lastKnownPlayerPosition);
                    }
                    break;



                case State.Investigating:
                    Debug.Log("INVESTIGATING: distance to last known position = " + Vector2.Distance(transform.position, lastKnownPlayerPosition));
                    navMeshAgent.isStopped = false;
                    float distanceToLastPos = Vector2.Distance(transform.position, lastKnownPlayerPosition);
                    if (distanceToLastPos < 0.2f)
                    {
                        state = State.Roaming;
                        roamingTime = roamingTimerMax;
                    }
                    else
                    {
                        navMeshAgent.SetDestination(lastKnownPlayerPosition);
                    }
                    DetectPlayer();
                    break;
            }

            if (enemyVisual != null)
            {
                Vector2 lookDir = Vector2.zero;

                if (state == State.Chasing)
                    lookDir = (player.position - transform.position).normalized;
                else if (state == State.Investigating)
                    lookDir = (lastKnownPlayerPosition - transform.position).normalized;

                if (lookDir != Vector2.zero)
                {
                    Vector2 roundedDir = Utils.GetEightDirection(lookDir);
                    enemyVisual.UpdateLookDirection(roundedDir);
                }
            }
        }
    }


    private void TryAttack()
    {
        Debug.Log("Attack started");
        isAttacking = true;
        navMeshAgent.isStopped = true;
        lastAttackTime = Time.time;

        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        Vector2 roundedDir = Utils.GetEightDirection(dirToPlayer);

        enemyVisual.animator.SetBool("IsMoving", false);

        enemyVisual.animator.SetFloat("Horizontal", roundedDir.x);
        enemyVisual.animator.SetFloat("Vertical", roundedDir.y);
        enemyVisual.animator.SetInteger("LastHorizontal", (int)roundedDir.x);
        enemyVisual.animator.SetInteger("LastVertical", (int)roundedDir.y);

        enemyVisual.SetAttackDirection(roundedDir);

        enemyVisual.StartAttack();
    }

    public void EndAttack()
    {
        Debug.Log("Attack ended");
        isAttacking = false;
        navMeshAgent.isStopped = false;
    }

    private void Roaming()
    {
        roamPosition = GetRoamingPosition();
        navMeshAgent.SetDestination(roamPosition);
    }

    private Vector3 GetRoamingPosition()
    {
        return startingPosition + Utils.GetRandomDir() * Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange && CanSeePlayer())
        {
            Debug.Log("Player spotted! Start chasing!");
            lastKnownPlayerPosition = player.position;
            state = State.Chasing;
        }
    }

    private bool CanSeePlayer()
    {
        Vector2 origin = transform.position;
        Vector2 target = player.position;

        RaycastHit2D hit = Physics2D.Linecast(origin, target, obstacleMask);
        return hit.collider == null;
    }
}
