using System;

using Robocode;

using RC.Math;
using RC.Behaviour;
using static RC.Logger;

namespace RC.Body
{
    public class SafeDistanceFromEnemyState : Body.State
    {
        private TrackedEnemy Enemy = null;

        public SafeDistanceFromEnemyState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {

        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            // Stop approaching if too close
            if (Strategy.IsEnemyCloseEnough(behaviour.Robot, Enemy))
            {
                behaviour.ChangeBodyState(new Body.RotateAroundEnemyState(Enemy, true));
                return;
            }

            Vector2 robotXY = new Vector2(behaviour.Robot.X, behaviour.Robot.Y);
            Vector2 robotEnemyVectorNorm = (Enemy.Position - robotXY).GetNormalized();

            Double safeDistance = Enemy.Distance - Strategy.GetSafeDistance(behaviour.Robot);
            Vector2 targetXY = robotXY + robotEnemyVectorNorm * safeDistance;

            behaviour.Robot.GoToPosition(targetXY);
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}