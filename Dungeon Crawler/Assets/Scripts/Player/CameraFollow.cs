using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;         // ÷ель Ч игрок
    [SerializeField] private Vector2 minPosition;      // ћинимальные координаты (левый нижний угол карты)
    [SerializeField] private Vector2 maxPosition;      // ћаксимальные координаты (правый верхний угол карты)

    private float camHalfHeight;
    private float camHalfWidth;

    private void Start()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
        Application.targetFrameRate = 60;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float clampedX = Mathf.Clamp(target.position.x, minPosition.x + camHalfWidth, maxPosition.x - camHalfWidth);
        float clampedY = Mathf.Clamp(target.position.y, minPosition.y + camHalfHeight, maxPosition.y - camHalfHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
