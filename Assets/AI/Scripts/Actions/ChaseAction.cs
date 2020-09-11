using MK.AI;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Chase", menuName = "MK/AI/Actions/Chase Action")]
public class ChaseAction : Action
{
    public float avoidDistance = 2f;
    public float checkRadius = 0.5f;

    private const int COLLISION_CHECK_COUNT = 360;

    public override void Act(StateController controller)
    {     
        EnemyController enemyController = (EnemyController)controller;
        if (enemyController && enemyController.chaseTarget != null)
        {         
            Vector3 dirToTarget = (enemyController.chaseTarget.position - enemyController.transform.position).normalized;

            Vector3 checkDir = dirToTarget;

            for (int i = 0; i < COLLISION_CHECK_COUNT; i++)
            {
                RaycastHit2D hit = Physics2D.CircleCast(enemyController.transform.position, checkRadius, checkDir, avoidDistance, enemyController.obastacleMask);
                if (hit.collider != null)
                {
                    checkDir = Quaternion.AngleAxis(enemyController.GeneratedTurningDir, Vector3.forward) * checkDir;
                }
                else
                {
                    break;
                }
            }


            enemyController.Movement.direction = checkDir;
        }
    }
}
