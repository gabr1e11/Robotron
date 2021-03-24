using System;

using Robocode.Util;

using RC.Behaviour;
using RC.Physics;

namespace RC.Radar
{
    public class TrackEnemyLostScanState : Radar.State
    {
        TrackedEnemy Enemy = null;

        public TrackEnemyLostScanState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            if (Enemy.Time == behaviour.Robot.Time)
            {
                behaviour.ChangeRadarState(new Radar.TrackEnemyLockFocusState(Enemy));
                return;
            }

            behaviour.Robot.SetTurnRadarRight(Constants.MaxRadarMovementPerTurnDegrees);
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }

    }
}