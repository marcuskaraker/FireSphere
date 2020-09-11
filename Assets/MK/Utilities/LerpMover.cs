using UnityEngine;

namespace MK
{
    public class LerpMover : MonoBehaviour
    {
        public Vector3 localTargetPosition;
        public float lerpSpeed = 10f;
        public float snapMultiplier = 1f;

        private void Update()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, localTargetPosition, Time.deltaTime * lerpSpeed);
        }

        public void SnapInDirection(string direction)
        {
            Vector2 newPos = Vector2.zero;

            switch (direction.ToLower())
            {
                case "up":
                    newPos = Vector2.up;
                    break;
                case "down":
                    newPos = Vector2.down;
                    break;
                case "left":
                    newPos = Vector2.left;
                    break;
                case "right":
                    newPos = Vector2.right;
                    break;
            }

            transform.localPosition = newPos * snapMultiplier;
        }
    }
}

