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

        public RotateAroundEnemyState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(Robotron robot)
        {

        }

        public void Execute(Robotron robot)
        {
            robot.RotateAroundPosition(Enemy.Position, Utils.ToRadians(10.0), true);
        }

        public void Exit(Robotron robot)
        {

        }
    }
}