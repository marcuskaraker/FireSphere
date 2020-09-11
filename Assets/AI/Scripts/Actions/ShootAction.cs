using MK.AI;
using UnityEngine;

[CreateAssetMenu(fileName = "Action_Shoot", menuName = "MK/AI/Actions/Shoot Action")]
public class ShootAction : Action
{
    public float viewCircleRadius = 0.5f;
    public float range = 10f;

    public override void Act(StateController controller)
    {
        EnemyController enemyController = (EnemyController)controller;
        if (enemyController && enemyController.chaseTarget != null)
        {
            RaycastHit2D hit = Physics2D.CircleCast(enemyController.transform.position, viewCircleRadius, enemyController.transform.right, range, enemyController.interactionMask);
            if (hit.collider != null)
            {
                enemyController.Shooter.Shoot();
            }         
        }
    }
}
