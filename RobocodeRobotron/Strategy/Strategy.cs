using System;
using System.Collections.Generic;
using System.Linq;

using static RC.Logger;
using RC.Math;
using Robocode;

namespace RC
{
    static public class Strategy
    {
        // Enables/disables the use of the danger score
        public const bool UseDangerScore = true;

        // Minimum distance change to choose another enemy that is closer
        public const Double MinDistanceChange = 0.0;

        // Maximum number of turns to consider for danger score
        public const long MaxBulletHitTimeDiff = 16 * 4;

        // Minimum energy an enemy has to have for us to ram into it
        public const Double MinEnergyForRamming = 40.0;

        // Safe distance to keep from an enemy
        static public bool IsEnemyCloseEnough(Robotron player, TrackedEnemy enemy)
        {
            return enemy.Distance <= Strategy.GetSafeDistance(player);
        }

        static public bool ShouldRamEnemy(Robotron player, TrackedEnemy enemy)
        {
            return ((player.Energy * player.Energy) > 2 * (enemy.Energy * enemy.Energy)) &&
                (enemy.Energy < Strategy.MinEnergyForRamming);
        }

        static public Double GetSafeDistance(Robotron robot)
        {
            return 3.0 * robot.Width + Physics.Constants.MaxTankMovementPerTurn;
        }

        static public TrackedEnemy CalculateTrackedEnemy(TrackedEnemy currentEnemy, Robotron robot)
        {
            TrackedEnemy trackedEnemy = null;

            if (robot.TrackedEnemies.GetEnemies().Count == 0)
            {
                return null;
            }

            List<TrackedEnemy> enemiesDistance = robot.TrackedEnemies.GetEnemies().Values.ToList<TrackedEnemy>();
            List<TrackedEnemy> enemiesDanger = robot.TrackedEnemies.GetEnemies().Values.ToList<TrackedEnemy>();

            enemiesDistance.Sort((itemA, itemB) => itemA.Distance.CompareTo(itemB.Distance));
            enemiesDanger.Sort((itemA, itemB) => itemB.DangerScore.CompareTo(itemA.DangerScore));

            Double MaxDangerScore = enemiesDanger.ElementAt(0).DangerScore;

            // Select the closest that is not the most dangerous
            foreach (TrackedEnemy enemy in enemiesDistance)
            {
                if (!robot.IsTeammate(enemy.Name) && (!UseDangerScore || MaxDangerScore == 0.0 || enemy.DangerScore < MaxDangerScore))
                {
                    trackedEnemy = enemy;
                    break;
                }
            }

            if (currentEnemy == null || !robot.TrackedEnemies.GetEnemies().ContainsKey(currentEnemy.Name))
            {
                return trackedEnemy;
            }
            else if (trackedEnemy.Distance < (currentEnemy.Distance - MinDistanceChange))
            {
                return trackedEnemy;
            }
            else
            {
                return currentEnemy;
            }
        }

        static public Vector2 CalculateAntigravity(Robotron robot, TrackedEnemy trackedEnemy)
        {
            Vector2 antigravity = new Vector2();

            foreach (KeyValuePair<String, TrackedEnemy> pair in robot.TrackedEnemies.GetEnemies())
            {
                if (trackedEnemy != null && trackedEnemy == pair.Value)
                {
                    // For the tracked enemy instead go towards it
                    Vector2 gravityVectorNorm = new Vector2(trackedEnemy.Position.X - robot.X, trackedEnemy.Position.Y - robot.Y) * 0.8;

                    // TODO: Adjust amounnf of movement, don't get too close
                    gravityVectorNorm *= 0.8;

                    antigravity += gravityVectorNorm;
                }
                else
                {
                    antigravity += pair.Value.AntigravityVector;
                }
            }

            Log("Antigravity is " + antigravity);
            return antigravity;
        }

        static public void SmartFire(Robotron robot, TrackedEnemy enemy)
        {
            double firePower = 0.1;
            double enemyDistance = Util.CalculateDistance(robot.X, robot.Y, enemy.Position.X, enemy.Position.Y);

            if (enemyDistance > 500.0)
            {
                firePower = 1;
            }
            else if (enemyDistance < 20.0)
            {
                firePower = 3.0;
            }
            else
            {
                firePower = 1.0 + 2.0 * (500.0 - enemyDistance) / 480.0;
            }

            Log("Firing Enemy: " + enemy.Name + " with power: " + firePower + "(Gun heat = " + robot.GunHeat + ")");
            robot.Fire(firePower);
        }

        static public bool IsPositionSafeFromWalls(Robotron robot, Vector2 position)
        {
            return (position.X < (robot.BattleFieldWidth - robot.Width - 10)) && (position.X > robot.Width + 10) &&
                   (position.Y < (robot.BattleFieldHeight - robot.Height - 10)) && (position.Y > robot.Height + 10);
        }
    }
}
