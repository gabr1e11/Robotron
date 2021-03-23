using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC
{
    // Enemy
    [Serializable()]
    public class TeamEvent
    {
        public String Name;
        public Enemy Enemy;

        public TeamEvent(String name, Enemy enemy)
        {
            Name = name;
            Enemy = enemy;
        }
    }
}