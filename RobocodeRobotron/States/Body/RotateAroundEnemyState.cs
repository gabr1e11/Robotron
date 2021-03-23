using System;

using Robocode;
using Robocode.Util;

using RC.Math;
using static RC.Logger;

namespace RC.Body
{
    public class RotateAroundEnemyState : Body.State
    {
        private TrackedEnemy Enemy = null;
        private bool ClockwiseTurn = true;

        public RotateAroundEnemyState(TrackedEnemy enemy, bool clockwise)
        {
            Enemy = enemy;
            ClockwiseTurn = clockwise;
        }

        public void Enter(Robotron robot)
        {

        }

        public void Execute(Robotron robot)
        {
            robot.RotateAroundPosition(Enemy.Position, Utils.ToRadians(10.0), ClockwiseTurn);
        }

        public void Exit(Robotron robot)
        {

        }
    }
}