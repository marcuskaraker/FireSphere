using MK.AI;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Chase", menuName = "MK/AI/Actions/Chase Action")]
public class ChaseAction : Action
{
    public float avoidDistance = 2f;
    public float checkRadius = 0.5f;
    public bool sineMovement;
    public float sineAffectorSpeed = 1f;
    public float sineAffectorMagnitude = 1f;

    private const int COLLISION_CHECK_COUNT = 180;
    private const int TURNING_ANGLE = 2;

    public override void Act(StateController controller)
    {     
        EnemyController enemyController = (EnemyController)controller;
        if (enemyController && enemyController.chaseTarget != null)
        {         
            Vector3 dirToTarget = (enemyController.chaseTarget.position - enemyController.transform.position).normalized;

            if (sineMovement)
            {
                Vector3 perpendicular = Vector3.Cross(dirToTarget, Vector3.forward).normalized;
                dirToTarget += perpendicular * Mathf.Sin(Time.time * sineAffectorSpeed) * sineAffectorMagnitude;
                //Debug.Log(Mathf.Sin(Time.time * sineAffectorSpeed)) * sineAffectorMagnitude;
            }

            Vector3 checkDir = dirToTarget;

            for (int i = 0; i < COLLISION_CHECK_COUNT; i++)
            {
                RaycastHit2D hit = Physics2D.CircleCast(enemyController.transform.position, checkRadius, checkDir, avoidDistance, enemyController.obastacleMask);
                if (hit.collider != null)
                {
                    checkDir = Quaternion.AngleAxis(enemyController.GeneratedTurningDir * TURNING_ANGLE, Vector3.forward) * checkDir;
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
