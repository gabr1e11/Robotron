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
            behaviour.ChangeBodyState(new Body.SafeDistanceFromEnemyState(Enemy));
            behaviour.ChangeGunState(new Gun.TrackEnemyState(Enemy));
            behaviour.ChangeRadarState(new Radar.TrackEnemyLockFocusState(Enemy));

            Player.SendTeamEvent(TeamEventType.TrackingEnemy, new Enemy(Enemy));
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            // Enemy tracking
            TrackedEnemy newTrackedEnemy = Strategy.CalculateTrackedEnemy(Enemy, Player);
            if (newTrackedEnemy == null)
            {
                behaviour.ChangeState(new WaitForTrackedEnemyState(Player));
                return;
            }
            if (newTrackedEnemy != Enemy)
            {
                behaviour.ChangeState(new AttackEnemyState(Player, newTrackedEnemy));
                return;
            }

            // Enemy ramming
            if (Player.IsFlagSet(Robotron.EventFlags.BumpedEnemy))
            {
                Player.ClearFlag(Robotron.EventFlags.BumpedEnemy);
                behaviour.ChangeState(new WaitForTrackedEnemyState(Player));
                return;
            }

            // Enemy ramming
            if (Strategy.ShouldRamEnemy(Player, Enemy))
            {
                behaviour.ChangeState(new RamEnemyState(Player, Enemy));
                return;
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
