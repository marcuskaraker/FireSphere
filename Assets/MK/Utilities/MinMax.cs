using UnityEngine;

namespace MK
{
    [System.Serializable]
    public struct MinMax
    {
        public float min;
        public float max;

        public Vector2 Vector { get { return new Vector2(min, max); } }

        public MinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}