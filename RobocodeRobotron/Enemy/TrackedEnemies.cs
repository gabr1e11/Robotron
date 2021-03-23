using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;

namespace RC
{
    public class TrackedEnemies
    {
        private Robotron Player = null;
        private Dictionary<String, TrackedEnemy> Enemies;

        public TrackedEnemies(Robotron player)
        {
            Player = player;
            Enemies = new Dictionary<string, TrackedEnemy>(Player.Others);
        }

        public void Update()
        {
            foreach (KeyValuePair<String, TrackedEnemy> pair in Enemies)
            {
                pair.Value.UpdateForCurrentTurn(Player);
            }
        }

        public Dictionary<String, TrackedEnemy> GetEnemies()
        {
            return Enemies;
        }

        public void OnScannedRobot(Enemy enemy)
        {
            if (Player.IsTeammate(enemy.Name))
            {
                return;
            }

            Log("Enemy " + enemy.Name + " detected at distance " + enemy.Distance);
            if (!Enemies.ContainsKey(enemy.Name))
            {
                Enemies[enemy.Name] = new TrackedEnemy(Player, enemy);
            }

            Enemies[enemy.Name].UpdateFromRadar(Player, enemy);
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
