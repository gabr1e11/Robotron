using System;

using Robocode.Util;

using RC.Behaviour;
using RC.Physics;

namespace RC.Radar
{
    public class TrackEnemySwipeAroundState : Radar.State
    {
        TrackedEnemy Enemy = null;
        int FullSwipeCount = (int)System.Math.Ceiling(360.0 / Constants.MaxRadarMovementPerTurnDegrees);

        public TrackEnemySwipeAroundState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            if (FullSwipeCount <= 0)
            {
                behaviour.ChangeRadarState(new TrackEnemyLockFocusState(Enemy));
                return;
            }

            behaviour.Robot.SetTurnRadarRight(Constants.MaxRadarMovementPerTurnDegrees);

            FullSwipeCount--;
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }

    }
}