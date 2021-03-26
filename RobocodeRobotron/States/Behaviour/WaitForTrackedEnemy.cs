using System;

using Robocode;

namespace RC.Behaviour
{
    public class WaitForTrackedEnemyState : Behaviour.State
    {
        private Robotron Player = null;
        private TrackedEnemy EnemyToAvoid = null;

        public WaitForTrackedEnemyState(Robotron player, TrackedEnemy enemyToAvoid = null)
        {
            Player = player;
            EnemyToAvoid = enemyToAvoid;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            behaviour.ChangeRadarState(new Radar.FullScanState());
            behaviour.ChangeGunState(new Gun.NoopState());
            behaviour.ChangeBodyState(new Body.AntigravityState(null));

            Player.SendTeamEvent(TeamEventType.SearchingForEnemy, null);
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            TrackedEnemy trackedEnemy = Strategy.CalculateTrackedEnemy(Player, EnemyToAvoid);
            if (trackedEnemy != null)
            {
                behaviour.ChangeState(new AttackEnemyState(Player, trackedEnemy));
                return;
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
