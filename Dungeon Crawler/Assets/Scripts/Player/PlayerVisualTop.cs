using UnityEngine;

public class PlayerVisualTop : MonoBehaviour
{
    private Animator animator;
    public Player player;

    public Transform target;
    public Vector3 positionOffset = new Vector3(0, 1f, 0);
    [SerializeField] public float followSpeed = 10f;
    [SerializeField] public float maxOffset = 0.3f;

    private Vector3 baseLocalOffset;

    public static PlayerVisualTop Instance { get; private set; }

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

        Vector3 nextPos = Vector3.Lerp(currentWorldPos, targetWorldPos, followSpeed * Time.deltaTime);

        Vector3 offset = nextPos - targetWorldPos;
        if (offset.magnitude > maxOffset)
            nextPos = targetWorldPos + offset.normalized * maxOffset;

        transform.position = nextPos;
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
        followSpeed = 10f;
        maxOffset = 0.5f;
    }
}
