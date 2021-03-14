using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC.Behaviour
{
    public class TrackingEnemyState : FSM.FSMState<BehaviourStateMachine>
    {
        private Robotron Robot = null;
        private TrackedEnemy TrackedEnemy = null;

        public TrackingEnemyState(Robotron robot)
        {
            Robot = robot;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            TrackedEnemy = Strategy.CalculateTrackedEnemy(Robot.TrackedEnemies);

            behaviour.ChangeBodyState(new Body.AntigravityState(TrackedEnemy));
            behaviour.ChangeGunState(new Gun.TrackEnemyState(TrackedEnemy));
            behaviour.ChangeRadarState(new Radar.FullScanState());
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            TrackedEnemy newTrackedEnemy = Strategy.CalculateTrackedEnemy(Robot.TrackedEnemies);
            if (newTrackedEnemy != TrackedEnemy)
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
