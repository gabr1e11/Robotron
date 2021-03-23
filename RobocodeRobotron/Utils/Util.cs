using System;

using Robocode.Util;
using RC.Math;

namespace RC
{
    public static class Util
    {
        private static readonly Random RandomGen = new Random();

        public static Vector2 CalculateXYPos(double startX, double startY, double bearingRadians, double distance)
        {
            // Sin is used for X and Cos for Y to transform to Robocode angle system
            double enemyX = startX + distance * System.Math.Sin(bearingRadians);
            double enemyY = startY + distance * System.Math.Cos(bearingRadians);

            return new Vector2(enemyX, enemyY);
        }

        public static double CalculateBearingRadians(double startX, double startY, double endX, double endY)
        {
            return System.Math.Atan2(endX - startX, endY - startY);
        }

        public static double CalculateBearingRadians(double X, double Y)
        {
            return System.Math.Atan2(X, Y);
        }

        public static bool GetRandomBool()
        {
            return Convert.ToBoolean(RandomGen.Next() % 2);
        }

        public static double GetRandom()
        {
            return RandomGen.NextDouble();
        }

        public static double CalculateDistance(double startX, double startY, double endX, double endY)
        {
            double deltaX = endX - startX;
            double deltaY = endY - startY;

            return System.Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        static public int GetTeamBotNumber(String name)
        {
            String n = "1";
            int low = name.IndexOf("(") + 1;
            int hi = name.LastIndexOf(")");
            if (low >= 0 && hi >= 0) { n = name.Substring(low, hi - low); }
            return Int32.Parse(n);
        }
    }
}
