using System;

using Robocode;

namespace RC.Behaviour
{
    public class WaitForTrackedEnemyState : Behaviour.State
    {
        private Robotron Robot = null;

        public WaitForTrackedEnemyState(Robotron robot)
        {
            Robot = robot;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            behaviour.ChangeRadarState(new Radar.FullScanState());
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            TrackedEnemy trackedEnemy = Strategy.CalculateTrackedEnemy(null, Robot.TrackedEnemies, Robot.Time);
            if (trackedEnemy != null)
            {
                behaviour.ChangeState(new TrackEnemyState(Robot));
                return;
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
