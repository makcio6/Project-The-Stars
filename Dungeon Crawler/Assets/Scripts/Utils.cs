using UnityEngine;

namespace Crawler.Utils
{
    public static class Utils
    {

        public static Vector3 GetRandomDir() {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        public static Vector2 GetEightDirection(Vector2 dir)
        {
            dir.Normalize();

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle = (angle + 360f) % 360f;

            if (angle >= 337.5f || angle < 22.5f)
                return Vector2.right;
            else if (angle >= 22.5f && angle < 67.5f)
                return new Vector2(1, 1);
            else if (angle >= 67.5f && angle < 112.5f)
                return Vector2.up;
            else if (angle >= 112.5f && angle < 157.5f)
                return new Vector2(-1, 1);
            else if (angle >= 157.5f && angle < 202.5f)
                return Vector2.left;
            else if (angle >= 202.5f && angle < 247.5f)
                return new Vector2(-1, -1);
            else if (angle >= 247.5f && angle < 292.5f)
                return Vector2.down;
            else // angle >= 292.5f && angle < 337.5f
                return new Vector2(1, -1);
        }

    }
    
}
