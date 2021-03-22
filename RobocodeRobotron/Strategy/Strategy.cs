using System;
using System.Collections.Generic;

using static RC.Logger;
using RC.Math;

namespace RC
{
    static public class Strategy
    {
        public const Double MinDistanceChange = 0.0;
        public const long MaxBulletHitTimeDiff = 16 * 4;
        public const Double DangerScoreThreshold = 0.2;

        static public TrackedEnemy CalculateTrackedEnemy(TrackedEnemy CurrentEnemy, TrackedEnemies trackedEnemies, long time)
        {
            Double maxDangerScore = Double.MinValue;
            Double minDistance = Double.MaxValue;

            TrackedEnemy trackedEnemy = null;

            foreach (KeyValuePair<String, TrackedEnemy> pair in trackedEnemies.GetEnemies())
            {
                if (pair.Value.Distance < minDistance)
                {
                    minDistance = pair.Value.Distance;
                    trackedEnemy = pair.Value;
                }

                // Calculate danger score
                /*Double dangerScore = 0.0;
                if (pair.Value.DamageToPlayer.Count > 0)
                {
                    foreach (TrackedEnemy.EnemyDamage enemyDamage in pair.Value.DamageToPlayer)
                    {
                        dangerScore += enemyDamage.Damage;
                    }
                    dangerScore /= pair.Value.DamageToPlayer.Count;
                    dangerScore = 1.0;
                }
                else
                {
                    dangerScore = 1.0;
                }
                dangerScore /= pair.Value.Distance;

                if (maxDangerScore <= 0.0 || dangerScore > (maxDangerScore + DangerScoreThreshold))
                {
                    trackedEnemy = pair.Value;
                    maxDangerScore = dangerScore;
                }*/
            }

            if (CurrentEnemy == null || !trackedEnemies.GetEnemies().ContainsKey(CurrentEnemy.Name))
            {
                return trackedEnemy;
            }
            else if (trackedEnemy.Distance < (CurrentEnemy.Distance - MinDistanceChange))
            {
                return trackedEnemy;
            }
            else
            {
                return CurrentEnemy;
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
