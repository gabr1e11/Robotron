using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Robocode;
using Robocode.Util;

namespace RC
{
    // Robotron
    //
    // Robot for Picanhas competition
    //
    class Robotron : AdvancedRobot
    {
        private readonly Double MinAngle = 10.0;
        private readonly Double MaxAngle = 90.0;

        private Double AngleRange = 0.0;
        private Double DoubleAngleRange = 0.0;

        private Dictionary<String, TrackedEnemy> trackedEnemies;

        private TrackedEnemy curTrackedEnemy = null;

        private void Init()
        {
            AngleRange = MaxAngle - MinAngle;
            DoubleAngleRange = 2 * AngleRange;

            SetColors(Color.Red, Color.OrangeRed, Color.LightGoldenrodYellow);

            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            IsAdjustRadarForRobotTurn = true;

            trackedEnemies = new Dictionary<string, TrackedEnemy>(Others);
        }

        private Boolean movingForward = true;
        private void UpdateTank()
        {
            /*if (curTrackedEnemy != null)
            {
                SetAhead(0.0);
                SetTurnRight(0.0);
                return;
            }*/

            if (movingForward)
            {
                if (DistanceRemaining == 0.0)
                {
                    SetAhead(Util.GetRandom() * 100.0 + 100.0);
                    movingForward = false;
                }
            }
            else
            {
                if (DistanceRemaining == 0.0)
                {
                    SetBack(Util.GetRandom() * 100.0 + 100.0);
                    movingForward = true;
                }
            }
            SetTurnRight(360.0);
        }

        //
        // Gun methods
        //
        private void PointGunAt(TrackedEnemy curTrackedEnemy)
        {
            double enemyBearingRadians = Util.CalculateBearingRadians(X, Y, curTrackedEnemy.X, curTrackedEnemy.Y);
            double gunRotationRadians = Utils.NormalRelativeAngle(enemyBearingRadians - GunHeadingRadians);

            Out.WriteLine("Gun heading " + GunHeading);
            Out.WriteLine("Enemy relative bearing " + Utils.ToDegrees(enemyBearingRadians));

            Out.WriteLine("Rotating gun by " + gunRotationRadians + " radians (" + Utils.ToDegrees(gunRotationRadians) + " degrees)");
            SetTurnGunRightRadians(gunRotationRadians);
        }

        private void UpdateGun()
        {
            // Calculate closest enemy
            Double minDistance = Double.MaxValue;
            foreach (KeyValuePair<String, TrackedEnemy> pair in trackedEnemies)
            {
                double enemyDistance = Util.CalculateDistance(X, Y, pair.Value.X, pair.Value.Y);

                if (enemyDistance < minDistance)
                {
                    minDistance = enemyDistance;

                    Out.WriteLine("Tracking enemy " + pair.Value.Name + " at distance " + minDistance);
                    curTrackedEnemy = pair.Value;
                }
            }

            // If we are tracking an enemy, move the gun, then fire
            if (curTrackedEnemy != null)
            {
                Out.WriteLine("Pointing gun at enemy " + curTrackedEnemy.Name);
                PointGunAt(curTrackedEnemy);

                if (Math.Abs(GunTurnRemaining) < 0.5)
                {
                    SmartFire(curTrackedEnemy);
                }
            }
        }

        //
        // Radar methods
        //
        private void UpdateRadar()
        {
            // For now just rotate it at max speed
            Out.WriteLine("Rotating radar by 45 degrees");
            SetTurnRadarRight(45);
        }

        private void SmartFire(TrackedEnemy enemy)
        {
            double firePower = 0.1;
            double enemyDistance = Util.CalculateDistance(X, Y, enemy.X, enemy.Y);

            if (enemyDistance > 500.0)
            {
                firePower = 0.1;
            }
            else if (enemyDistance < 20.0)
            {
                firePower = 3.0;
            }
            else
            {
                firePower = 3.0 * (500.0 - enemyDistance) / 480.0;
            }

            Out.WriteLine("Firing Enemy: " + enemy.Name + " with power: " + firePower + "(Gun heat = " + GunHeat + ")");
            Fire(firePower);
        }

        public override void Run()
        {
            base.Run();

            Init();

            while (true)
            {
                UpdateTank();
                UpdateRadar();
                UpdateGun();

                Execute();
            }
        }

        //
        // When a robot sees another robot
        //
        public override void OnScannedRobot(ScannedRobotEvent enemy)
        {
            base.OnScannedRobot(enemy);

            Out.WriteLine("Enemy " + enemy.Name + " detected at distance " + enemy.Distance);
            trackedEnemies[enemy.Name] = new TrackedEnemy(this, enemy);

            /* if (curTrackedEnemy != null && curTrackedEnemy.Name == enemy.Name)
             {
                 if (Math.Abs(RadarHeading - GunHeading) <= 3.0f)
                 {
                     Out.WriteLine("  Firing current tracked enemy " + curTrackedEnemy.Name);
                     SmartFire(enemy);
                 }
             }*/
        }

        //
        // When another robot dies
        //
        public override void OnRobotDeath(RobotDeathEvent enemy)
        {
            base.OnRobotDeath(enemy);

            Out.WriteLine("Enemy " + enemy.Name + " died");
            if (curTrackedEnemy.Name == enemy.Name)
            {
                curTrackedEnemy = null;
            }

            trackedEnemies.Remove(enemy.Name);
        }

    }
}