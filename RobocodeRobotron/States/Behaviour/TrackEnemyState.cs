using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC.Behaviour
{
    public class TrackEnemyState : Behaviour.State
    {
        private Robotron Robot = null;
        private TrackedEnemy Enemy = null;

        public TrackEnemyState(Robotron robot)
        {
            Robot = robot;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            Enemy = Strategy.CalculateTrackedEnemy(Robot.TrackedEnemies);

            behaviour.ChangeBodyState(new Body.TrackEnemyApproachState(Enemy));
            behaviour.ChangeGunState(new Gun.TrackEnemyState(Enemy));
            behaviour.ChangeRadarState(new Radar.FullScanState());
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            TrackedEnemy newTrackedEnemy = Strategy.CalculateTrackedEnemy(Robot.TrackedEnemies);
            if (newTrackedEnemy != Enemy)
            {
                behaviour.ChangeState(new TrackEnemyState(Robot));
                return;
            }

            Double SafeDistanceThreshold = 5.0 * Robot.Width + Physics.Constants.MaxTankMovementPerTurn;

            if (Enemy.Distance <= SafeDistanceThreshold)
            {
                behaviour.ChangeBodyState(new Body.TrackEnemyRotateAroundState(Enemy));
                return;
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
