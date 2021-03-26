using System;
using System.Collections.Generic;
using System.Linq;

using Robocode;

using static RC.Logger;

namespace RC
{
    public class TrackedBullets
    {
        private Robotron Player = null;
        private List<TrackedBullet> Bullets;

        public TrackedBullets(Robotron player)
        {
            Player = player;
            Bullets = new List<TrackedBullet>();
        }

        public void Update(long Time)
        {
            Bullets.RemoveAll(item => (Time - item.HitTime) > Strategy.Config.BulletHistoryMaxTurns);
        }

        public List<TrackedBullet> GetBullets()
        {
            return Bullets;
        }

        public void OnBulletHit(BulletHitEvent evnt)
        {
            Bullets.Add(new TrackedBullet(evnt));
        }
    }
}
