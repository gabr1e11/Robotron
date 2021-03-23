using System;

using Robocode.Util;

namespace RC.Radar
{
    public class TrackEnemyState : Radar.State
    {
        TrackedEnemy Enemy = null;

        public TrackEnemyState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(Robotron robot)
        {

        }

        public void Execute(Robotron robot)
        {
            robot.SetTurnRadarRightRadians(2 * Utils.NormalRelativeAngle(robot.HeadingRadians + Enemy.BearingRadians - robot.RadarHeadingRadians));
        }

        public void Exit(Robotron robot)
        {

        }

    }
}