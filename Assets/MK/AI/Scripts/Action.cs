using UnityEngine;

namespace MK.AI
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Act(StateController controller);
    }
}

