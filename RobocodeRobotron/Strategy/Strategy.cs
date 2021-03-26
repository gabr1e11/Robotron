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
        public static Config Config { get; private set; }

        static public void SetConfig(Config config)
        {
            Config = config;
        }

        // Team members initial positions
        static public Vector2 GetTeamMemberInitPos(Robotron player)
        {
            int index = Util.GetTeamBotNumber(player.Name);
            return new Vector2(index * player.BattleFieldWidth / (player.Teammates.Length + 2.0), player.BattleFieldHeight / 2.0);
        }

        static public bool IsInMemberQuadrant(Robotron player, Vector2 position)
        {
            int index = Util.GetTeamBotNumber(player.Name);

            Double quadrantWidth = player.BattleFieldWidth / (player.Teammates.Length + 1.0);

            return (position.X >= ((index - 1) * quadrantWidth)) &&
                   (position.X < (index * quadrantWidth));
        }

        static public bool IsInitPosCloseEnough(Robotron player)
        {
            Vector2 targetPos = GetTeamMemberInitPos(player);
            Vector2 currentPos = new Vector2(player.X, player.Y);

            return (targetPos - currentPos).Module() <= Config.InitPosAllowedDistance;
        }

        // Safe distance to keep from an enemy
        static public bool IsEnemyCloseEnough(Robotron player, TrackedEnemy enemy)
        {
            return enemy.Distance <= Config.EnemyCloseEnoughMaxMultiplier * Strategy.GetSafeDistance(player) &&
                enemy.Distance >= Config.EnemyCloseEnoughMinMultiplier * Strategy.GetSafeDistance(player);
        }

        // Safe distance to keep from an enemy
        static public bool IsEnemyTooFar(Robotron player, TrackedEnemy enemy)
        {
            return enemy.Distance >= Config.EnemyTooFarMultiplier * Strategy.GetSafeDistance(player);
        }

        static public bool IsEnemyTooClose(Robotron player, TrackedEnemy enemy)
        {
            return enemy.Distance < Config.EnemyTooCloseMultiplier * Strategy.GetSafeDistance(player);
        }

        static public bool ShouldRamEnemy(Robotron player, TrackedEnemy enemy)
        {
            return ((player.Energy * player.Energy) > 2 * (enemy.Energy * enemy.Energy)) &&
                (enemy.Energy < Config.MinEnergyForRamming);
        }

        static public Double GetSafeDistance(Robotron robot)
        {
            if (robot.Energy < Config.SafeDistanceLowEnergyThreshold)
            {
                return Config.SafeDistanceMultiplierLowEnergy * robot.Width + Physics.Constants.MaxTankMovementPerTurn;
            }
            else
            {
                return Config.SafeDistanceMultiplierNormalEnergy * robot.Width + Physics.Constants.MaxTankMovementPerTurn;
            }
        }

        static public TrackedEnemy CalculateTrackedEnemy(Robotron robot, TrackedEnemy enemyToAvoid = null)
        {
            List<TrackedEnemy> enemies = robot.TrackedEnemies.GetEnemies();
            List<String> blacklistedEnemies = robot.TrackedEnemies.GetBlacklistedEnemies();

            // Blacklist enemies if there is more than one left
            if (enemies.FindAll(item => !robot.IsTeammate(item.Name)).Count > 1)
            {
                enemies.RemoveAll(item => blacklistedEnemies.Contains(item.Name));
            }

            if (enemies.Count == 0)
            {
                return null;
            }

            // Calculate tracking score
            foreach (TrackedEnemy enemy in enemies)
            {
                enemy.TrackingScore = Config.TrackingScoreDistanceWeight * enemy.Distance +
                Config.TrackingScoreDangerWeight * enemy.DangerScore +
                Config.TrackingScoreEnergyWeight * enemy.Energy;

                if (IsInMemberQuadrant(robot, enemy.Position))
                {
                    enemy.TrackingScore *= 3.0;
                }
            }
            enemies.Sort((itemA, itemB) => itemA.TrackingScore.CompareTo(itemB.TrackingScore));

            if (enemyToAvoid != null &&
                enemies.ElementAt(0).Name == enemyToAvoid.Name &&
                enemies.Count > 1)
            {
                return enemies.ElementAt(1);
            }
            return enemies.ElementAt(0);
        }

        static public Vector2 CalculateAntigravity(Robotron robot, TrackedEnemy trackedEnemy)
        {
            Vector2 antigravity = new Vector2();

            foreach (TrackedEnemy enemy in robot.TrackedEnemies.GetEnemies())
            {
                if (trackedEnemy != null && trackedEnemy == enemy)
                {
                    // For the tracked enemy instead go towards it
                    Vector2 gravityVectorNorm = new Vector2(trackedEnemy.Position.X - robot.X, trackedEnemy.Position.Y - robot.Y) * 0.8;

                    // TODO: Adjust amounnt of movement, don't get too close
                    gravityVectorNorm *= 0.8;

                    antigravity += gravityVectorNorm;
                }
                else
                {
                    antigravity += enemy.AntigravityVector;
                }
            }

            Log("Antigravity is " + antigravity);
            return antigravity;
        }

        static public Double CalculateFirePower(Robotron robot, TrackedEnemy enemy)
        {
            double firePower = 0.1;
            double enemyDistance = Util.CalculateDistance(robot.X, robot.Y, enemy.Position.X, enemy.Position.Y);

            if (enemyDistance > Config.MaxDistanceLowEnergyBullet)
            {
                firePower = 1;
            }
            else if (enemyDistance < Config.MinDistanceHighEnergyBullet)
            {
                firePower = 3.0;
            }
            else
            {
                firePower = 1.0 + 2.0 * (Config.MaxDistanceLowEnergyBullet - enemyDistance) / (Config.MaxDistanceLowEnergyBullet - Config.MinDistanceHighEnergyBullet);
            }

            return firePower;
        }

        static public bool IsPositionSafeFromWalls(Robotron robot, Vector2 position)
        {
            return (position.X < (robot.BattleFieldWidth - robot.Width - 10)) && (position.X > robot.Width + 10) &&
                   (position.Y < (robot.BattleFieldHeight - robot.Height - 10)) && (position.Y > robot.Height + 10);
        }

        static public bool IsAllyUnderFriendlyFire(Robotron robot)
        {
            List<TrackedBullet> hitAllies = robot.TrackedBullets.GetBullets().FindAll(
                item => robot.IsTeammate(item.VictimName) &&
                        item.HitTime <= Strategy.Config.AllyFireCheckMaxTurns);

            return hitAllies.Count >= Strategy.Config.AllyFireCheckMaxHits;
        }
    }
}
