using System;

namespace RC.Math
{
    [Serializable()]
    public struct Vector2
    {
        public double X;
        public double Y;

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator +(Vector2 a)
          => a;

        public static Vector2 operator -(Vector2 a)
          => new Vector2(-a.X, -a.Y);

        public static Vector2 operator +(Vector2 a, Vector2 b)
          => new Vector2(a.X + b.X, a.Y + b.Y);

        public static Vector2 operator -(Vector2 a, Vector2 b)
          => a + (-b);

        public static Vector2 operator *(Vector2 a, double factor)
          => new Vector2(factor * a.X, factor * a.Y);

        public double Module()
        {
            return System.Math.Sqrt(X * X + Y * Y);
        }

        public Vector2 GetNormalized()
        {
            Vector2 normVector = new Vector2(X, Y);
            normVector.Normalize();
            return normVector;
        }

        public void Normalize()
        {
            double module = Module();

            X /= module;
            Y /= module;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }
}
