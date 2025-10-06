using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    public Player player;

    public Transform target;
    public Vector3 positionOffset = new Vector3(0, 0, 0);
    [SerializeField] public float followSpeed = 6f;
    [SerializeField] public float maxOffset = 0.8f;

    [SerializeField] private float normalFollowSpeed = 10f;
    [SerializeField] private float normalMaxOffset = 0.5f;

    [SerializeField] private float dashFollowSpeed = 2f;
    [SerializeField] private float dashMaxOffset = 1.5f;

    [SerializeField] private float inertiaLerpSpeed = 4f;

    private float currentFollowSpeed;
    private float currentMaxOffset;

    private bool isInDash = false;

    private Vector3 baseLocalOffset;

    public static PlayerVisual Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        animator = GetComponent<Animator>();

        baseLocalOffset = transform.localPosition;
    }

    private void Update()
    {
        if (player != null)
            animator.SetBool("IsMoving", player.IsMoving);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetWorldPos = target.position + positionOffset;
        Vector3 currentWorldPos = transform.position;

        Vector3 nextPos = Vector3.Lerp(currentWorldPos, targetWorldPos, currentFollowSpeed * Time.deltaTime);

        Vector3 offset = nextPos - targetWorldPos;
        if (offset.magnitude > currentMaxOffset)
            nextPos = targetWorldPos + offset.normalized * currentMaxOffset;

        transform.position = nextPos;

        currentFollowSpeed = Mathf.Lerp(currentFollowSpeed, isInDash ? dashFollowSpeed : normalFollowSpeed, Time.deltaTime * inertiaLerpSpeed);
        currentMaxOffset = Mathf.Lerp(currentMaxOffset, isInDash ? dashMaxOffset : normalMaxOffset, Time.deltaTime * inertiaLerpSpeed);
    }


    public void BoostInertia()
    {
        followSpeed = 4f;
        maxOffset = 1.0f;

        CancelInvoke(nameof(ResetInertia));
        Invoke(nameof(ResetInertia), 0.3f);
    }

    void ResetInertia()
    {
        followSpeed = currentFollowSpeed;
        maxOffset = currentMaxOffset;
    }

    public void SetDashInertia(bool value)
    {
        isInDash = value;
    }
}