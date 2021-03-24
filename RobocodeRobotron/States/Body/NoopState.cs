using System;

using RC.Behaviour;

namespace RC.Body
{
    public class NoopState : Body.State
    {
        public void Enter(BehaviourStateMachine behaviour)
        {
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
        }

        public void Exit(BehaviourStateMachine behaviour)
        {
        }
    }
}