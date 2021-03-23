using System;

using Robocode;

namespace RC.Behaviour
{
    public class WaitForTrackedEnemyState : Behaviour.State
    {
        private Robotron Player = null;

        public WaitForTrackedEnemyState(Robotron player)
        {
            Player = player;
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            behaviour.ChangeRadarState(new Radar.FullScanState());
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            TrackedEnemy trackedEnemy = Strategy.CalculateTrackedEnemy(null, Player);
            if (trackedEnemy != null)
            {
                behaviour.ChangeState(new ApproachEnemyState(Player, trackedEnemy));
                return;
            }
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
