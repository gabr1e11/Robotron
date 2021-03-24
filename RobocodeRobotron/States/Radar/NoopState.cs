using System;

using RC.Behaviour;

namespace RC.Radar
{
    public class NoopState : Radar.State
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