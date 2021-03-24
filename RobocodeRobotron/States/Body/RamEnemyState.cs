using System;

using Robocode;
using Robocode.Util;

using RC.Math;
using RC.Behaviour;
using static RC.Logger;

namespace RC.Body
{
    public class RamEnemyState : Body.State
    {
        private TrackedEnemy Enemy = null;

        public RamEnemyState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {

        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            behaviour.Robot.GoToPosition(Enemy.Position);
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}