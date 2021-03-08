using System;

using Robocode;

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
        public Vector2 Position { get; private set; }
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

            Console.WriteLine("Enemy X,Y is at position " + Position);
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

            Console.WriteLine("Enemy " + Name + " antigravity is" + AntigravityVector);

            // Distance
            Distance = Util.CalculateDistance(me.X, me.Y, Position.X, Position.Y);
        }
    }
}