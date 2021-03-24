using System;

using RC.Math;
using RC.Behaviour;
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

        public void Enter(BehaviourStateMachine behaviour)
        {

        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            // Tank
            Vector2 robotXY = new Vector2(behaviour.Robot.X, behaviour.Robot.Y);
            Vector2 antigravityVector = Strategy.CalculateAntigravity(behaviour.Robot, Enemy);
            Vector2 antigravityNewPosition = robotXY + antigravityVector;
            double antigravityDistance = antigravityVector.Module();

            Vector2 newPosition = new Vector2(behaviour.Robot.X, behaviour.Robot.Y);
            if (antigravityDistance >= 5.0)
            {
                Log("Using antigravity");
                newPosition = robotXY + antigravityVector;
            }
            else
            {
                Log("Using random");

                // Just go somewhere
                double newX = behaviour.Robot.X + Util.GetRandom() * 20.0 - 10.0;
                double newY = behaviour.Robot.Y + Util.GetRandom() * 20.0 - 10.0;

                newPosition = new Vector2(newX, newY);
            }

            Log("Tank movement -> CurPos=" + new Vector2(behaviour.Robot.X, behaviour.Robot.Y) + " NewPos=" + newPosition);

            // Check for walls
            if (Strategy.IsPositionSafeFromWalls(behaviour.Robot, newPosition))
            {
                Log("Position is safe from walls");
                behaviour.Robot.GoToPosition(newPosition);
            }
            else
            {
                Log("Position is NOT safe from walls, stopping tank");
                behaviour.Robot.StopTank();
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}