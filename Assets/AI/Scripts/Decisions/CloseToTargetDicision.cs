using UnityEngine;
using MK.AI;

[CreateAssetMenu(fileName = "Decision_CloseToTarget", menuName = "MK/AI/Decision/Close Target Decision")]
public class CloseToTargetDicision : Decision
{
    public float minSquaredDistance = 6f;

    public override bool Decide(StateController controller)
    {
        if (controller.chaseTarget != null)
        {
            return (controller.transform.position - controller.chaseTarget.position).sqrMagnitude < minSquaredDistance;
        }

        return false;
    }
}
