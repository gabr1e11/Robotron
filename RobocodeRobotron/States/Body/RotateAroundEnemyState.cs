using System;

using Robocode;
using Robocode.Util;

using RC.Math;
using static RC.Logger;
using RC.Behaviour;

namespace RC.Body
{
    public class RotateAroundEnemyState : Body.State
    {
        private TrackedEnemy Enemy = null;
        private bool ClockwiseTurnEnabled = true;
        private long LastRotationChangeTurn = 0;

        public RotateAroundEnemyState(TrackedEnemy enemy, bool clockwise)
        {
            Enemy = enemy;
            ClockwiseTurnEnabled = clockwise;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {

        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            // Go to the safe distance again
            if (Strategy.IsEnemyTooFar(behaviour.Robot, Enemy) ||
                Strategy.IsEnemyTooClose(behaviour.Robot, Enemy))
            {
                behaviour.ChangeBodyState(new SafeDistanceFromEnemyState(Enemy));
                return;
            }

            // Change direction if hit a wall
            if (behaviour.Robot.IsFlagSet(Robotron.EventFlags.HitWall) && (behaviour.Robot.Time - LastRotationChangeTurn > 10))
            {
                ClockwiseTurnEnabled = !ClockwiseTurnEnabled;
                LastRotationChangeTurn = behaviour.Robot.Time;
            }

            Double angularSpeedRadiansSec = Utils.ToRadians(Strategy.Config.RotationAroundEnemySpeedDegreesSec);
            behaviour.Robot.RotateAroundPosition(Enemy.Position, angularSpeedRadiansSec, ClockwiseTurnEnabled);
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}