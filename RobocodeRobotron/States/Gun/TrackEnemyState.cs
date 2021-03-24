using System;

using static RC.Logger;

using RC.Behaviour;

namespace RC.Gun
{
    public class TrackEnemyState : Gun.State
    {
        private TrackedEnemy Enemy = null;

        public TrackEnemyState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {

        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            behaviour.Robot.PointGunAt(Enemy);

            if (System.Math.Abs(behaviour.Robot.GunTurnRemaining) < 7.0)
            {
                Double firePower = Strategy.CalculateFirePower(behaviour.Robot, Enemy);

                Log("Firing Enemy: " + Enemy.Name + " with power: " + firePower + "(Gun heat = " + behaviour.Robot.GunHeat + ")");
                behaviour.Robot.Fire(firePower);
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}