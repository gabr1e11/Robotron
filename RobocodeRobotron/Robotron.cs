using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Robocode;
using Robocode.Util;

namespace rcano
{
    class Robotron : AdvancedRobot
    {
        private readonly Random RandomGen = new Random();

        private readonly Double MinAngle = 10.0;
        private readonly Double MaxAngle = 90.0;

        private Double AngleRange = 0.0;
        private Double DoubleAngleRange = 0.0;

        private Dictionary<String, ScannedRobotEvent> trackedEnemies;

        private void Init()
        {
            AngleRange = MaxAngle - MinAngle;
            DoubleAngleRange = 2 * AngleRange;

            SetColors(Color.Red, Color.OrangeRed, Color.LightGoldenrodYellow);

            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            IsAdjustRadarForRobotTurn = true;

            trackedEnemies = new Dictionary<string, ScannedRobotEvent>(Others);
        }

        private Boolean movingForward = true;
        private void UpdateTank()
        {
            if (movingForward)
            {
                if (DistanceRemaining == 0.0)
                {
                    SetAhead(100.0);
                    movingForward = false;
                }
            }
            else
            {
                if (DistanceRemaining == 0.0)
                {
                    SetBack(100.0);
                    movingForward = true;
                }
            }
            SetTurnRight(360.0f);
        }

        private void UpdateGun()
        {
            // Calculate closest enemy
            Double distance = Double.MaxValue;
            foreach (KeyValuePair<String, ScannedRobotEvent> pair in trackedEnemies)
            {
                if (pair.Value.Distance < distance)
                {
                    distance = pair.Value.Distance;

                    Out.WriteLine("Tracking enemy " + pair.Value.Name + " at distance " + distance);
                    trackedEnemy = pair.Value;
                }
            }

            // If we are tracking an enemy, move the gun, then fire
            if (trackedEnemy != null)
            {
                Double gunTurnDegrees = Utils.NormalRelativeAngleDegrees(trackedEnemy.Bearing + Heading - GunHeading);

                Out.WriteLine("Tracking enemy " + trackedEnemy.Name + " at bearing " + trackedEnemy.Bearing);
                Out.WriteLine("  Gun is at heading " + GunHeading);
                Out.WriteLine("  Tank is at heading " + Heading);
                Out.WriteLine("  Moving gun " + gunTurnDegrees + " degree");

                if (Math.Abs(gunTurnDegrees) < 8.0)
                {
                    SmartFire(trackedEnemy);
                }

                SetTurnGunRight(gunTurnDegrees);
            }
        }

        private void UpdateRadar()
        {
            // For now just rotate it at max speed
            SetTurnRadarRight(45);
        }


        private void SmartFire(ScannedRobotEvent e)
        {
            Double firePower = 0.1;

            if (e.Distance > 500.0)
            {
                firePower = 0.1;
            }
            else if (e.Distance < 20.0)
            {
                firePower = 3.0;
            }
            else
            {
                firePower = 3.0 * (500.0 - e.Distance) / 480.0;
            }

            Out.WriteLine("Firing Enemy: " + e.Name + " with power: " + firePower);
            Fire(firePower);
        }

        private Boolean GetRandomBool()
        {
            return Convert.ToBoolean(RandomGen.Next() % 2);
        }

        public override void Run()
        {
            Init();

            while (true)
            {
                UpdateTank();
                UpdateRadar();
                UpdateGun();

                Execute();
            }
        }

        private ScannedRobotEvent trackedEnemy = null;

        // Robot event handler, when the robot sees another robot
        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            base.OnScannedRobot(e);

            Out.WriteLine("Enemy Detected: " + e.Name + " at distance " + e.Distance);
            trackedEnemies[e.Name] = e;

            /* if (trackedEnemy != null && e.Name == trackedEnemy.Name)
             {
                 // Check for the arc compared to the size of the enemy to see if we will hit
                 if (Math.Abs(GunHeading - trackedEnemy.Bearing + Heading) < 5.0)
                 {
                     SmartFire(e);
                 }
             }*/
        }

        public override void OnRobotDeath(RobotDeathEvent evnt)
        {
            base.OnRobotDeath(evnt);

            trackedEnemies.Remove(evnt.Name);
        }

    }
}