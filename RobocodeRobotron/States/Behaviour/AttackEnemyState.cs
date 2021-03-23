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
        private bool ClockwiseTurnEnabled = true;
        private long LastRotationChangeTurn = 0;

        public AttackEnemyState(Robotron player, TrackedEnemy enemy)
        {
            Player = player;
            Enemy = enemy;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            behaviour.ChangeBodyState(new Body.RotateAroundEnemyState(Enemy, ClockwiseTurnEnabled));
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
                behaviour.ChangeState(new RamEnemyState(Player, Enemy));
                return;
            }

            if (Strategy.IsEnemyTooFar(Player, Enemy))
            {
                behaviour.ChangeState(new ApproachEnemyState(Player, Enemy));
                return;
            }

            if (Player.IsFlagSet(Robotron.EventFlags.HitWall) && (Player.Time - LastRotationChangeTurn > 10))
            {
                ClockwiseTurnEnabled = !ClockwiseTurnEnabled;

                behaviour.ChangeBodyState(new Body.RotateAroundEnemyState(Enemy, ClockwiseTurnEnabled));

                LastRotationChangeTurn = Player.Time;

                Player.ClearFlag(Robotron.EventFlags.HitWall);
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
