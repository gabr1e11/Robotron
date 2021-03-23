using System;
using System.Collections.Generic;
using System.Linq;

using Robocode;

using static RC.Logger;

namespace RC
{
    public class TrackedEnemies
    {
        private Robotron Player = null;
        private Dictionary<String, TrackedEnemy> Enemies;
        private List<String> BlacklistedEnemies;

        public TrackedEnemies(Robotron player)
        {
            Player = player;
            Enemies = new Dictionary<String, TrackedEnemy>(Player.Others);

            BlacklistedEnemies = new List<String>();
        }

        public void Update()
        {
            foreach (KeyValuePair<String, TrackedEnemy> pair in Enemies)
            {
                pair.Value.UpdateForCurrentTurn(Player);
            }
        }

        public List<TrackedEnemy> GetEnemies()
        {
            List<TrackedEnemy> enemies = Enemies.Values.ToList<TrackedEnemy>();

            enemies.RemoveAll(item => BlacklistedEnemies.Contains(item.Name));

            return enemies;
        }

        public void AddBlacklistedEnemy(Enemy enemy)
        {
            BlacklistedEnemies.Add(enemy.Name);
        }

        public void RemoveBlacklistedEnemy(Enemy enemy)
        {
            BlacklistedEnemies.Remove(enemy.Name);
        }

        public void ClearBlacklistedEnemies()
        {
            BlacklistedEnemies.Clear();
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
