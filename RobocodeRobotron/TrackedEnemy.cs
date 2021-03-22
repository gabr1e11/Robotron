using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC
{
    // TrackedEnemy
    //
    // Contains information of an enemy seen by the radar
    //
    public class TrackedEnemy
    {
        public struct EnemyDamage
        {
            public long Time;
            public Double Damage;

            public EnemyDamage(long time, Double damage)
            {
                Time = time;
                Damage = damage;
            }
        }

        public String Name { get; private set; }
        public double HeadingRadians { get; private set; } = 0.0;
        public double Energy { get; private set; } = 0.0;
        public double Velocity { get; private set; } = 0.0;
        public Vector2 Position { get; private set; } = new Vector2();
        public long LastTurnSeen { get; private set; } = 0;

        public Vector2 AntigravityVector { get; private set; }
        public Double Distance { get; private set; }
        public List<EnemyDamage> DamageToPlayer;

        public TrackedEnemy(AdvancedRobot me, ScannedRobotEvent enemy)
        {
            Name = enemy.Name;
            DamageToPlayer = new List<EnemyDamage>();

            UpdateFromRadar(me, enemy);
        }

        public void UpdateFromRadar(AdvancedRobot me, ScannedRobotEvent enemy)
        {
            HeadingRadians = enemy.HeadingRadians;
            Energy = enemy.Energy;
            Velocity = enemy.Velocity;

            Position = Util.CalculateXYPos(me.X, me.Y, enemy.BearingRadians + me.HeadingRadians, enemy.Distance);

            Log("Enemy " + Name + " is at position " + Position);
            Log("  My position is " + new Vector2(me.X, me.Y));
            LastTurnSeen = enemy.Time;

            UpdateFromPlayer(me);
        }

        public void UpdateFromPlayer(AdvancedRobot me)
        {
            // Antigravity
            double gForce = 500000.0;

            Vector2 playerPos = new Vector2(me.X, me.Y);
            Vector2 playerEnemyVector = Position - playerPos;

            double distance = playerEnemyVector.Module();
            double distance2 = distance * distance;
            double forceStrength = gForce / distance2;

            AntigravityVector = -playerEnemyVector.GetNormalized() * forceStrength;

            Log("Enemy " + Name + " antigravity is" + AntigravityVector);

            // Distance
            Distance = Util.CalculateDistance(me.X, me.Y, Position.X, Position.Y);
        }

        public void UpdateHitPlayer(HitByBulletEvent evnt)
        {
            Double damage = 4 * evnt.Bullet.Power;
            if (evnt.Bullet.Power > 1.0)
            {
                damage += 2.0 * (evnt.Bullet.Power - 1.0);
            }

            DamageToPlayer.Add(new EnemyDamage(evnt.Time, damage));

            DamageToPlayer.RemoveAll(item => (evnt.Time - item.Time) > Strategy.MaxBulletHitTimeDiff);
        }
    }
}