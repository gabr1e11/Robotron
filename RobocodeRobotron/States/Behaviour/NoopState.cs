using System;

namespace RC.Behaviour
{
    public class NoopState : Behaviour.State
    {
        public void Enter(BehaviourStateMachine behaviour)
        {
            behaviour.ChangeBodyState(new Body.NoopState());
            behaviour.ChangeGunState(new Gun.NoopState());
            behaviour.ChangeRadarState(new Radar.NoopState());
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
        }

        public void Exit(BehaviourStateMachine behaviour)
        {
        }
    }
}