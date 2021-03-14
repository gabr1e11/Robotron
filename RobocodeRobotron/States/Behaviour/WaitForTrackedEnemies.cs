using System;

using RC.FSM;

namespace RC.Behaviour
{
    public class WaitForTrackedEnemiesState : FSMState<BehaviourStateMachine>
    {
        private Robotron Robot = null;

        public WaitForTrackedEnemiesState(Robotron robot)
        {
            Robot = robot;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            behaviour.ChangeRadarState(new Radar.FullScanState());
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            TrackedEnemy trackedEnemy = Strategy.CalculateTrackedEnemy(Robot.TrackedEnemies);
            if (trackedEnemy != null)
            {
                behaviour.ChangeState(new TrackingEnemyState(Robot));
                return;
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
