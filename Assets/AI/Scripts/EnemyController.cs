using UnityEngine;
using MK.AI;
using MK;

public class EnemyController : StateController
{
    public GameObject target;
    public LayerMask interactionMask;
    public LayerMask obastacleMask;

    public MinMax speedMultiplierMinMax = new MinMax(0.8f, 1.2f);

    public float GeneratedTurningDir { get; private set; }
    public float GeneratedSpeedMultiplier { get; private set; }

    public Movement Movement { get; private set; }
    public Shooter Shooter { get; private set; }

    private void Awake()
    {
        // Get movment reference
        Movement = target.GetComponent<Movement>();
        Shooter = target.GetComponent<Shooter>();     

        GeneratedTurningDir = Random.Range(0, 2) == 0 ? -1 : 1;
        GeneratedSpeedMultiplier = Random.Range(speedMultiplierMinMax.min, speedMultiplierMinMax.max);

        Movement.speed *= GeneratedSpeedMultiplier;
    }

    private void Update()
    {
        UpdateStateController();

        if (chaseTarget)
        {
            Movement.rotationTarget = chaseTarget.position;
        }
    }
}
