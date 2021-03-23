using System;

using static RC.Logger;

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

            if (System.Math.Abs(robot.GunTurnRemaining) < 7.0)
            {
                Double firePower = Strategy.CalculateFirePower(robot, Enemy);

                Log("Firing Enemy: " + Enemy.Name + " with power: " + firePower + "(Gun heat = " + robot.GunHeat + ")");
                robot.Fire(firePower);
            }
        }

        public void Exit(Robotron robot)
        {

        }
    }
}