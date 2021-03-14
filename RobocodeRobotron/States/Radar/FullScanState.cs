using System;

namespace RC.Radar
{
    public class FullScanState : Radar.State
    {
        public void Enter(Robotron robot)
        {

        }

        public void Execute(Robotron robot)
        {
            robot.SetTurnRadarRight(45.0);
        }

        public void Exit(Robotron robot)
        {

        }

    }
}