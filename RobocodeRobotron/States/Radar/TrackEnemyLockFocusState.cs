using System;

using Robocode.Util;

using RC.Behaviour;

namespace RC.Radar
{
    public class TrackEnemyLockFocusState : Radar.State
    {
        TrackedEnemy Enemy = null;
        Double SwipeAreaHalfAngle = Strategy.Config.RadarScanAreaSwipeAngleRadians / 2.0;
        int LockFocusCount = Strategy.Config.LockRadarFocusMaxTurns;

        public TrackEnemyLockFocusState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            if (LockFocusCount <= 0)
            {
                behaviour.ChangeRadarState(new TrackEnemySwipeAroundState(Enemy));
                return;
            }

            if (Enemy.Time != behaviour.Robot.Time)
            {
                behaviour.ChangeRadarState(new Radar.TrackEnemyLostScanState(Enemy));
                return;
            }

            behaviour.Robot.PointRadarAt(Enemy, SwipeAreaHalfAngle);

            LockFocusCount--;
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }

    }
}