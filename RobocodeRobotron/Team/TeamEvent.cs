using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC
{
    public enum TeamEventType
    {
        OnScannedRobot
    }

    [Serializable()]
    public class TeamEvent
    {
        public TeamEventType Type;
        public Enemy Enemy;

        public TeamEvent(TeamEventType type, Enemy enemy)
        {
            Type = type;
            Enemy = enemy;
        }
    }
}