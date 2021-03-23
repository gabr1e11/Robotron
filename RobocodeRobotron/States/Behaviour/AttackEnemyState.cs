using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC.Behaviour
{
    public class AttackEnemyState : Behaviour.State
    {
        private Robotron Player = null;
        private TrackedEnemy Enemy = null;

        public AttackEnemyState(Robotron player, TrackedEnemy enemy)
        {
            Player = player;
            Enemy = enemy;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            behaviour.ChangeBodyState(new Body.RotateAroundEnemyState(Enemy));
            behaviour.ChangeGunState(new Gun.TrackEnemyState(Enemy));
            behaviour.ChangeRadarState(new Radar.FullScanState());
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            TrackedEnemy newTrackedEnemy = Strategy.CalculateTrackedEnemy(Enemy, Player);
            if (newTrackedEnemy != Enemy)
            {
                behaviour.ChangeState(new ApproachEnemyState(Player, newTrackedEnemy));
                return;
            }

            if (Strategy.ShouldRamEnemy(Player, Enemy))
            {
                behaviour.ChangeState(new RamEnemyState(Player, newTrackedEnemy));
                return;
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
