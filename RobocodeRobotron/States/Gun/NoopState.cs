using System;

using RC.Behaviour;

namespace RC.Gun
{
    public class NoopState : Gun.State
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