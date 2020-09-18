using UnityEngine;
using MK;
using UnityEngine.Events;
using System;

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

    [Range(1, 10)] public float sprintMultiplier;
    [Range(1, 10)] public float rotationSprintMultiplier;
    public float sprintAcceleration;

    public MovmentType movementType;
    public RotationType rotationType;

    public UnityEvent onStartSprint;
    public UnityEvent onEndSprint;

    private float appliedSprintMultiplier = 1f;
    private float appliedRotationSprintMultiplier = 1f;
    private bool isSprinting;

    // Rigidbody holder
    public Rigidbody2D Rb2D { get; private set; }

    public bool IsSprinting
    {
        get { return isSprinting; }
        set
        {
            if (isSprinting != value && value)
            {
                onStartSprint.Invoke();
            }
            else if (isSprinting != value && !value)
            {
                onEndSprint.Invoke();
            }

            isSprinting = value;
        }
    }

    private void Awake()
    {
        // Get rigidbody reference
        Rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isSprinting)
        {
            appliedSprintMultiplier = Mathf.Lerp(appliedSprintMultiplier, sprintMultiplier, Time.deltaTime * sprintAcceleration);
            appliedRotationSprintMultiplier = Mathf.Lerp(appliedRotationSprintMultiplier, rotationSprintMultiplier, Time.deltaTime * sprintAcceleration);
        }
        else
        {
            appliedSprintMultiplier = Mathf.Lerp(appliedSprintMultiplier, 1, Time.deltaTime * sprintAcceleration);
            appliedRotationSprintMultiplier = Mathf.Lerp(appliedRotationSprintMultiplier, 1, Time.deltaTime * sprintAcceleration);
        }     
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
        Rb2D.velocity = direction * speed * appliedSprintMultiplier;

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
        Rb2D.AddForce(transform.right * speed * appliedSprintMultiplier);

        Vector2 targetDir = (rotationTarget - Rb2D.position).normalized;
        if (rotationType == RotationType.RotateTowardsTarget)
        {
            float normalizedDir = Mathf.Sign(Vector2.SignedAngle(transform.right, targetDir));
            Rb2D.AddTorque(normalizedDir * rotationSpeed * appliedSprintMultiplier);
        }
        else if (rotationType == RotationType.RotateTowardsMovement)
        {
            float normalizedDir = Mathf.Sign(Vector2.SignedAngle(transform.right, direction));
            Rb2D.AddTorque(normalizedDir * rotationSpeed * appliedSprintMultiplier);
        }
    }
}