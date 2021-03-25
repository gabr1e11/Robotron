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
        // SAFE DISTANCE
        //   Multipliers based on the robot energy
        public Double SafeDistanceLowEnergyThreshold = 30.0;
        public Double SafeDistanceMultiplierLowEnergy = 5.0;
        public Double SafeDistanceMultiplierNormalEnergy = 3.0;
        //   Multipliers based on the enemy distance
        public Double EnemyTooFarMultiplier = 2.0;
        public Double EnemyTooCloseMultiplier = 0.5;
        public Double EnemyCloseEnoughMaxMultiplier = 1.5;
        public Double EnemyCloseEnoughMinMultiplier = 0.8;

        // BULLET ENERGY
        public Double MinDistanceHighEnergyBullet = 50.0;
        public Double MaxDistanceLowEnergyBullet = 500.0;

        // TRACKING SCORE (for selecting enemies)
        public Double TrackingScoreDistanceWeight = 1.0;
        public Double TrackingScoreDangerWeight = 0.0;
        public Double TrackingScoreEnergyWeight = 0.0;
        //    Maximum number of turns to consider for danger score
        public long MaxBulletHitTimeDiff = 16 * 4;

        // TEAM QUADRANT
        public Double InitPosAllowedDistance = 50.0f;

        // ENEMY LOCKED
        //    Angle of the radar swiping area
        public Double RadarScanAreaSwipeAngleDegrees = 20.0;
        //    Maximum number of turns that the radar will lock onto the enemy
        public int LockRadarFocusMaxTurns = 5;
        //    Angular speed for rotation around an enemy
        public Double RotationAroundEnemySpeedDegreesSec = 10.0;
        //    Enables/disables prediction of next enemy position
        public bool EnableEnemyPositionPrediction = true;
        // Minimum energy an enemy has to have for us to ram into it
        public Double MinEnergyForRamming = 40.0;
    }
}
