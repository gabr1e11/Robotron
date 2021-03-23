using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;

namespace RC
{
    public class TrackedEnemies
    {
        private Robotron Robot = null;
        private Dictionary<String, TrackedEnemy> Enemies;

        public TrackedEnemies(Robotron robot)
        {
            Robot = robot;
            Enemies = new Dictionary<string, TrackedEnemy>(Robot.Others);
        }

        public void Update()
        {
            foreach (KeyValuePair<String, TrackedEnemy> pair in Enemies)
            {
                pair.Value.UpdateForCurrentTurn(Robot);
            }
        }

        public Dictionary<String, TrackedEnemy> GetEnemies()
        {
            return Enemies;
        }

        public void OnScannedRobot(Enemy enemy)
        {
            Log("Enemy " + enemy.Name + " detected at distance " + enemy.Distance);
            if (!Enemies.ContainsKey(enemy.Name))
            {
                Enemies[enemy.Name] = new TrackedEnemy(Robot, enemy);
            }

            Enemies[enemy.Name].UpdateFromRadar(Robot, enemy);
        }

        public void OnRobotDeath(RobotDeathEvent enemy)
        {
            Log("Enemy " + enemy.Name + " died");
            Enemies.Remove(enemy.Name);
        }

        public void OnHitByBullet(HitByBulletEvent evnt)
        {
            if (Enemies.ContainsKey(evnt.Name))
            {
                Enemies[evnt.Name].UpdateHitPlayer(evnt);
            }

        }
    }
}
