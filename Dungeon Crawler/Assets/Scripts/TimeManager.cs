using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Range(0f, 5f)]
    public float TimeScale = 1f;

    private float originalFixedDelta;

    public float DeltaTime => Time.unscaledDeltaTime * TimeScale;
    public float FixedDeltaTime => originalFixedDelta * TimeScale;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        originalFixedDelta = Time.fixedDeltaTime;
    }
}
