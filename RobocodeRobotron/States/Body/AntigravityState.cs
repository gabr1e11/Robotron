using System;

using RC.Math;
using static RC.Logger;

namespace RC.Body
{
    public class AntigravityState : Body.State
    {
        private TrackedEnemy Enemy = null;

        public AntigravityState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(Robotron robot)
        {

        }

        public void Execute(Robotron robot)
        {
            // Tank
            Vector2 robotXY = new Vector2(robot.X, robot.Y);
            Vector2 antigravityVector = Strategy.CalculateAntigravity(robot, Enemy);
            Vector2 antigravityNewPosition = robotXY + antigravityVector;
            double antigravityDistance = antigravityVector.Module();

            Vector2 newPosition = new Vector2(robot.X, robot.Y);
            if (antigravityDistance >= 5.0)
            {
                Log("Using antigravity");
                newPosition = robotXY + antigravityVector;
            }
            else
            {
                Log("Using random");

                // Just go somewhere
                double newX = robot.X + Util.GetRandom() * 20.0 - 10.0;
                double newY = robot.Y + Util.GetRandom() * 20.0 - 10.0;

                newPosition = new Vector2(newX, newY);
            }

            Log("Tank movement -> CurPos=" + new Vector2(robot.X, robot.Y) + " NewPos=" + newPosition);

            // Check for walls
            if (Strategy.IsPositionSafeFromWalls(robot, newPosition))
            {
                Log("Position is safe from walls");
                robot.GoToPosition(newPosition);
            }
            else
            {
                Log("Position is NOT safe from walls, stopping tank");
                robot.StopTank();
            }
        }

        public void Exit(Robotron robot)
        {

        }
    }
}