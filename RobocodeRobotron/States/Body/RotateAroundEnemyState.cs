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
        private bool ClockwiseTurn = true;

        public RotateAroundEnemyState(TrackedEnemy enemy, bool clockwise)
        {
            Enemy = enemy;
            ClockwiseTurn = clockwise;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {

        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            behaviour.Robot.RotateAroundPosition(Enemy.Position, Utils.ToRadians(10.0), ClockwiseTurn);
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}