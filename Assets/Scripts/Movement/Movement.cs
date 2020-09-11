using UnityEngine;
using MK;
using UnityEngine.Events;

// Requirements
[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public enum RotationType { None, RotateTowardsTarget, RotateTowardsMovement }
    public enum MovmentType { Direct, Physics }

    // Movement specifics
    public Vector2 direction;
    public Vector2 rotationTarget;
    public float speed = 1f;
    public float rotationSpeed = 10f;

    public MovmentType movementType;
    public RotationType rotationType;

    // Rigidbody holder
    public Rigidbody2D Rb2D { get; private set; }

    private void Awake()
    {
        // Get rigidbody reference
        Rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        switch (movementType)
        {
            case MovmentType.Direct:
                DirectMovement();
                break;
            case MovmentType.Physics:
                PhysicsMovement();
                break;
        }
    }

    private void DirectMovement()
    {
        // Set velocity to rigidbody
        Rb2D.velocity = direction * speed;

        if (rotationType == RotationType.RotateTowardsTarget)
        {
            Rb2D.rotation = MKUtility.CalcDir2D(Rb2D.position, rotationTarget);
        }
        else if (rotationType == RotationType.RotateTowardsMovement)
        {
            Rb2D.rotation = MKUtility.CalcDir2D(direction);
        }
    }

    private void PhysicsMovement()
    {
        Rb2D.AddForce(transform.right * speed);

        Vector2 targetDir = (rotationTarget - Rb2D.position).normalized;
        if (rotationType == RotationType.RotateTowardsTarget)
        {
            float normalizedDir = Mathf.Sign(Vector2.SignedAngle(transform.right, targetDir));
            Rb2D.AddTorque(normalizedDir * rotationSpeed);
        }
        else if (rotationType == RotationType.RotateTowardsMovement)
        {
            float normalizedDir = Mathf.Sign(Vector2.SignedAngle(transform.right, direction));
            Rb2D.AddTorque(normalizedDir * rotationSpeed);
        }
    }

    public void AddForce()
    {

    }
}