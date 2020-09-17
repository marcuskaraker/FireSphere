using UnityEngine;

namespace MK
{
    public enum StraightDirection { Up, Down, Left, Right }

    public static class MKUtility
    {
        public static float CalcDir2D(Vector3 a, Vector3 b)
        {
            // Get Angle in Radians
            float AngleRad = Mathf.Atan2(b.y - a.y, b.x - a.x);
            // Get Angle in Degrees
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            // Rotate Object
            return AngleDeg;
        }

        public static float CalcDir2D(Vector3 dir)
        {
            // Get Angle in Radians
            float AngleRad = Mathf.Atan2(dir.y, dir.x);
            // Get Angle in Degrees
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            // Rotate Object
            return AngleDeg;
        }

        public static bool CheckIfLayerIsWithinMask(int layer, LayerMask mask)
        {
            // Bitwise operator for checking if a layer is within a mask. 
            // Unity Forums user: TowerOfBricks
            return (mask == (mask | (1 << layer)));
        }

        public static Vector2 GetStraightDirection(StraightDirection direction)
        {
            switch (direction)
            {
                case StraightDirection.Up:
                    return Vector2.up;
                case StraightDirection.Down:
                    return Vector2.down;
                case StraightDirection.Left:
                    return Vector2.left;
                case StraightDirection.Right:
                    return Vector2.right;
            }

            return Vector2.zero;
        }

        public static Vector3 GetRandomPositionInBounds(Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public static int NegativeModulo(int value, int mod)
        {
            int result = value % mod;
            if (result < 0)
            {
                result += mod;
            }

            return result;
        }
    }

    [System.Serializable]
    public struct MinMax
    {
        public float min;
        public float max;

        public MinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}

