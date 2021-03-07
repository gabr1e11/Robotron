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
        public String Name;
        public double HeadingRadians = 0.0;
        public double Energy = 0.0;
        public double Velocity = 0.0;
        public double X = 0.0f;
        public double Y = 0.0f;
        public long LastTurnSeen;

        public TrackedEnemy(AdvancedRobot me, ScannedRobotEvent enemy)
        {
            Name = enemy.Name;
            HeadingRadians = enemy.HeadingRadians;
            Energy = enemy.Energy;
            Velocity = enemy.Velocity;

            (X, Y) = Util.CalculateXYPos(me.X, me.Y, enemy.BearingRadians + me.HeadingRadians, enemy.Distance);

            Console.WriteLine("Enemy X,Y is (" + X + ", " + Y + ")");
            LastTurnSeen = enemy.Time;
        }
    }
}