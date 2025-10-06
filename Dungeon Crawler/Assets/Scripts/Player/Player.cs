using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private Rigidbody2D rb;
    private PlayerStats stats;

    private bool isMoving = false;
    public bool IsMoving => isMoving;

    private float finalMoveSpeed;

    private Vector2 inputVector = Vector2.zero;
    private float moveTimer = 0f;
    private float moveCooldown = 0.1f;
    private Vector2 lastPosition;

    private PlayerInputActions inputActions;

    public SwordSwing swordSwing;
    public float attackSlowDuration = 0.25f;
    private float attackSlowEndTime = 0f;
    private float lastAttackTime = -999f;
    private bool attackLeft = true;

    private float dashCooldownEndTime = 0f;
    private bool isDashing = false;
    private Vector2 dashDirection;
    private float dashStartTime;

    public Vector2 AimDirection => (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        inputActions.Player.Dash.performed += ctx => TryDash();
    }

    private void Start()
    {
        PlayerVisualTop.Instance.target = this.transform;
        PlayerVisual.Instance.target = this.transform;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        lastPosition = rb.position;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)
            && Time.time >= lastAttackTime + stats.attackCooldown
            && !swordSwing.IsSwinging)
        {
            Attack();
        }

        if (Input.GetMouseButton(0) || Time.time < attackSlowEndTime)
            finalMoveSpeed = stats.moveSpeedDuringAttack;
        else
            finalMoveSpeed = stats.moveSpeed;

        bool currentlyMoving = inputVector.sqrMagnitude > 0.01f;
        if (currentlyMoving)
        {
            moveTimer = moveCooldown;
            isMoving = true;
        }
        else
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f)
                isMoving = false;
        }
    }

    private void FixedUpdate()
    {
        inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        if (isDashing)
        {
            float elapsed = Time.time - dashStartTime;
            float dashProgress = elapsed / stats.dashDuration;

            if (dashProgress < 1f)
            {
                Vector2 dashVelocity = dashDirection * (stats.dashDistance / stats.dashDuration);
                rb.MovePosition(rb.position + dashVelocity * Time.fixedDeltaTime);
                PlayerVisual.Instance.SetDashInertia(true);
            }
            else
            {
                isDashing = false;
                PlayerVisual.Instance.SetDashInertia(false);
            }

            return;
        }

        if (IsInputBlocked())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (inputVector.sqrMagnitude > 0.01f)
        {
            Vector2 move = inputVector.normalized * finalMoveSpeed;
            rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
        }
    }

    private void TryDash()
    {
        if (inputVector != Vector2.zero && Time.time >= dashCooldownEndTime)
        {
            StartDash(inputVector);
        }
    }

    private void StartDash(Vector2 direction)
    {
        Debug.Log("StartDash!");
        isDashing = true;
        dashStartTime = Time.time;
        dashDirection = direction.normalized;
        dashCooldownEndTime = Time.time + stats.dashCooldown;

        PlayerVisual.Instance.BoostInertia();
    }

    private void Attack()
    {
        attackSlowEndTime = Time.time + stats.swingDuration;
        if (swordSwing.IsSwinging) return;

        Vector2 dir = AimDirection;
        swordSwing.StartSwing(attackLeft, dir);
        attackLeft = !attackLeft;
    }

    public void NotifyAttackFinished()
    {
        lastAttackTime = Time.time;
    }

    private bool IsInputBlocked()
    {
        return false;
    }
}
