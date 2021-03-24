using System;

using RC.Behaviour;

namespace RC.Radar
{
    public class FullScanState : Radar.State
    {
        public void Enter(BehaviourStateMachine behaviour)
        {

        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            behaviour.Robot.SetTurnRadarRight(45.0);
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }

    }
}