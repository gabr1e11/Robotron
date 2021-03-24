using System;
using System.Collections.Generic;

using Robocode;

using static RC.Logger;
using RC.Math;

namespace RC
{
    public class PositionTracker
    {
        private struct PositionInfo
        {
            public long Time;
            public Vector2 Position;

            public PositionInfo(long time, Vector2 position)
            {
                Time = time;
                Position = position;
            }
        }

        private List<PositionInfo> PositionHistory;
        private Vector2 LastKnownPosition;

        public PositionTracker()
        {
            PositionHistory = new List<PositionInfo>();
        }

        public void Update(long time)
        {
            PositionHistory.RemoveAll(item => (time - item.Time) > Strategy.Config.PositionHistoryMaxTurnsToKeep);
        }

        public void Add(long time, Vector2 position)
        {
            PositionHistory.Add(new PositionInfo(time, position));

            LastKnownPosition = position;
        }

        public Vector2 GetNextPosition()
        {
            return LastKnownPosition;

        }
    }
}