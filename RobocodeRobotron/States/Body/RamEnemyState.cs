using System;

using Robocode;
using Robocode.Util;

using RC.Math;
using static RC.Logger;

namespace RC.Body
{
    public class RamEnemyState : Body.State
    {
        private TrackedEnemy Enemy = null;

        public RamEnemyState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(Robotron robot)
        {

        }

        public void Execute(Robotron robot)
        {
            robot.GoToPosition(Enemy.Position);
        }

        public void Exit(Robotron robot)
        {

        }
    }
}