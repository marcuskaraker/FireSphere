using UnityEngine;
using UnityEngine.Events;

namespace MK.AI
{
    public class StateController : MonoBehaviour
    {
        [Header("State Controller")]
        public State currentState;
        public State remainState;

        public UnityEvent onExitState;

        [System.NonSerialized] public Transform chaseTarget;
        [System.NonSerialized] public float stateTimeElapsed;

        private bool aiActive = true;

        void Update()
        {
            UpdateStateController();
        }

        /// <summary>
        /// Updates the active state of the controller. Should be run in the Unity Update() method;
        /// </summary>
        public virtual void UpdateStateController()
        {
            if (!aiActive)
                return;
            currentState.UpdateState(this);
        }

        /// <summary>
        /// Transitions to the given state and calls OnExitState().
        /// </summary>
        public virtual void TransitionToState(State nextState)
        {
            if (nextState != remainState)
            {
                currentState = nextState;
                OnExitState();
            }
        }

        /// <summary>
        /// Checks if the controller has been in its state for the given duration.
        /// </summary>
        public virtual bool CheckIfCountDownElapsed(float duration)
        {
            stateTimeElapsed += Time.deltaTime;
            return (stateTimeElapsed >= duration);
        }

        private void OnExitState()
        {
            stateTimeElapsed = 0;
            onExitState.Invoke();
        }
    }
}