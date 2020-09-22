using UnityEngine;

namespace MK
{
    public enum StraightDirection { Up, Down, Left, Right }

    public static class MKUtility
    {
        public static float CalcDir2D(Vector3 a, Vector3 b)
        {
            float angleRadians = Mathf.Atan2(b.y - a.y, b.x - a.x);
            float angleDegrees = (180 / Mathf.PI) * angleRadians;
            return angleDegrees;
        }

        public static float CalcDir2D(Vector3 dir)
        {
            float angleRadians = Mathf.Atan2(dir.y, dir.x);
            float angleDegrees = (180 / Mathf.PI) * angleRadians;
            return angleDegrees;
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
            return GetRandomPositionInBounds(bounds, Vector3.zero);
        }

        public static Vector3 GetRandomPositionInBounds(Bounds bounds, Vector3 padding)
        {
            return new Vector3(
                Random.Range(bounds.min.x + padding.x, bounds.max.x - padding.x),
                Random.Range(bounds.min.y + padding.y, bounds.max.y - padding.y),
                Random.Range(bounds.min.z + padding.z, bounds.max.z - padding.z)
            );
        }

        /// <summary>
        /// Modulo that loops around on negative input.
        /// </summary>
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

    public static class IntegerExtensions
    {
        public static string WithSuffix(this int num)
        {
            string number = num.ToString();
            if (number.EndsWith("11")) return number + "th";
            if (number.EndsWith("12")) return number + "th";
            if (number.EndsWith("13")) return number + "th";
            if (number.EndsWith("1")) return number + "st";
            if (number.EndsWith("2")) return number + "nd";
            if (number.EndsWith("3")) return number + "rd";
            return number + "th";
        }
    }
}

