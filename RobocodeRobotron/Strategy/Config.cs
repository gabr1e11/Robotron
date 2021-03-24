using System;
using System.Collections.Generic;
using System.Linq;

using static RC.Logger;
using RC.Math;
using Robocode;

namespace RC
{
    public class Config
    {
        // Maximum number of turns to consider for danger score
        public long MaxBulletHitTimeDiff = 16 * 4;

        // Minimum energy an enemy has to have for us to ram into it
        public Double MinEnergyForRamming = 40.0;

        // Minimum distance for full energy bullet
        public Double MinDistanceHighEnergyBullet = 50.0;
        public Double MaxDistanceLowEnergyBullet = 500.0;

        // Weights for the tracking score formula
        public Double TrackingScoreDistanceWeight = 1.0;
        public Double TrackingScoreDangerWeight = 0.0;
        public Double TrackingScoreEnergyWeight = 0.0;

        // Team rules
        public Double InitPosAllowedDistance = 50.0f;

        // Radar scan area swipe angle
        public Double RadarScanAreaSwipeAngleRadians = 20.0;

        // Maximum number of turns that the radar will lock onto the enemy
        public int LockRadarFocusMaxTurns = 5;

        // Angular speed for rotation around an enemy
        public Double RotationAroundEnemySpeedDegreesSec = 10.0;

        // Maximum number of turns to keep for the position history of an enemy
        public long PositionHistoryMaxTurnsToKeep = 20;
    }
}
