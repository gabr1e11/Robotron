using System;

using RC.Math;

using Robocode;

namespace RC.Behaviour
{
    public class GoToQuadrantState : Behaviour.State
    {
        private Robotron Player = null;
        private Vector2 TargetInitPos;

        public GoToQuadrantState(Robotron player)
        {
            Player = player;

            TargetInitPos = Strategy.GetTeamMemberInitPos(Player);
        }

        public void Enter(BehaviourStateMachine behaviour)
        {
            behaviour.ChangeRadarState(new Radar.FullScanState());
        }

        public void Execute(BehaviourStateMachine behaviour)
        {
            if (Strategy.IsInitPosCloseEnough(Player) || Player.IsFlagSet(Robotron.EventFlags.BumpedEnemy))
            {
                Player.ClearFlag(Robotron.EventFlags.BumpedEnemy);
                behaviour.ChangeState(new WaitForTrackedEnemyState(Player));
                return;
            }

            Player.GoToPosition(TargetInitPos);
        }

        public void Exit(BehaviourStateMachine behaviour)
        {

        }
    }
}
