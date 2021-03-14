using System;

namespace RC.Gun
{
    public class TrackEnemyState : Gun.State
    {
        private TrackedEnemy Enemy = null;

        public TrackEnemyState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(Robotron robot)
        {

        }

        public void Execute(Robotron robot)
        {
            robot.PointGunAt(Enemy);

            if (System.Math.Abs(robot.GunTurnRemaining) < 0.5)
            {
                Strategy.SmartFire(robot, Enemy);
            }
        }

        public void Exit(Robotron robot)
        {

        }
    }
}