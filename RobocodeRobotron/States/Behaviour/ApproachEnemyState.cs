using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC.Behaviour
{
    public class ApproachEnemyState : Behaviour.State
    {
        private Robotron Player = null;
        private TrackedEnemy Enemy = null;

        public ApproachEnemyState(Robotron player, TrackedEnemy enemy)
        {
            Player = player;
            Enemy = enemy;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            behaviour.ChangeBodyState(new Body.ApproachEnemyState(Enemy));
            behaviour.ChangeGunState(new Gun.TrackEnemyState(Enemy));
            behaviour.ChangeRadarState(new Radar.TrackEnemyState(Enemy));

            Player.SendTeamEvent(TeamEventType.TrackingEnemy, new Enemy(Enemy));
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            TrackedEnemy newTrackedEnemy = Strategy.CalculateTrackedEnemy(Enemy, Player);
            if (newTrackedEnemy == null)
            {
                behaviour.ChangeState(new WaitForTrackedEnemyState(Player));
                return;
            }
            if (newTrackedEnemy != Enemy)
            {
                behaviour.ChangeState(new ApproachEnemyState(Player, newTrackedEnemy));
                return;
            }
            if (Player.IsFlagSet(Robotron.EventFlags.BumpedEnemy))
            {
                Player.ClearFlag(Robotron.EventFlags.BumpedEnemy);
                behaviour.ChangeState(new WaitForTrackedEnemyState(Player));
                return;
            }
            if (Strategy.IsEnemyCloseEnough(Player, Enemy))
            {
                behaviour.ChangeState(new Behaviour.AttackEnemyState(Player, Enemy));
                return;
            }
            if (Enemy.Time != Player.Time)
            {
                behaviour.ChangeRadarState(new Radar.FullScanState());
            }
            else
            {
                behaviour.ChangeRadarState(new Radar.TrackEnemyState(Enemy));
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
