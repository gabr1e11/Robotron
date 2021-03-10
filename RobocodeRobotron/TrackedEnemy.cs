using System;

using Robocode;

using static RC.Logger;

namespace RC
{
    // TrackedEnemy
    //
    // Contains information of an enemy seen by the radar
    //
    class TrackedEnemy
    {
        public String Name { get; private set; }
        public double HeadingRadians { get; private set; } = 0.0;
        public double Energy { get; private set; } = 0.0;
        public double Velocity { get; private set; } = 0.0;
        public Vector2 Position { get; private set; } = new Vector2();
        public long LastTurnSeen { get; private set; } = 0;

        public Vector2 AntigravityVector { get; private set; }
        public Double Distance { get; private set; }

        public TrackedEnemy(AdvancedRobot me, ScannedRobotEvent enemy)
        {
            Name = enemy.Name;

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
    }
}