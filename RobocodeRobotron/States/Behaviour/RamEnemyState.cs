﻿using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC.Behaviour
{
    public class RamEnemyState : Behaviour.State
    {
        private Robotron Player = null;
        private TrackedEnemy Enemy = null;

        public RamEnemyState(Robotron player, TrackedEnemy enemy)
        {
            Player = player;
            Enemy = enemy;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            behaviour.ChangeBodyState(new Body.RamEnemyState(Enemy));
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

            // TODO: Escape from enemy if they are about to kill us
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
