using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC
{
    // Enemy
    [Serializable()]
    public class Enemy
    {
        public String Name { get; private set; }
        public double HeadingRadians { get; private set; } = 0.0;
        public double BearingRadians { get; private set; } = 0.0;
        public double Energy { get; private set; } = 0.0;
        public double Velocity { get; private set; } = 0.0;
        public Vector2 Position { get; private set; } = new Vector2();
        public long Time { get; private set; } = 0;
        public Double Distance { get; private set; }

        public Enemy(Robotron player, ScannedRobotEvent enemy)
        {
            Name = enemy.Name;
            HeadingRadians = enemy.HeadingRadians;
            BearingRadians = enemy.BearingRadians;
            Energy = enemy.Energy;
            Velocity = enemy.Velocity;
            Time = enemy.Time;
            Distance = enemy.Distance;

            Position = Util.CalculateXYPos(player.X, player.Y, enemy.BearingRadians + player.HeadingRadians, enemy.Distance);
        }

    }
}