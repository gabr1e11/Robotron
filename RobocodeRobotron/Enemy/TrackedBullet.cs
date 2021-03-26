using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC
{
    public class TrackedBullet
    {
        public String VictimName { get; private set; }
        public Double VictimEnergy { get; private set; }
        public long HitTime = 0;

        public TrackedBullet(BulletHitEvent bulletEvent)
        {
            VictimName = bulletEvent.VictimName;
            VictimEnergy = bulletEvent.VictimEnergy;
            HitTime = bulletEvent.Time;
        }
    }
}