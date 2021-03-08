using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Numerics;

using Robocode;
using Robocode.Util;

namespace RC
{
    enum RadarState
    {
        Discovery
    }

    enum MoveState
    {
        Wander
    }

    // Robotron v0.2
    //
    // Robot for Picanhas competition
    //
    // v0.1
    //   - Created discovery radar (turns all the time)
    //   - Tracked all enemies
    //   - Point gun at closest enemy and fire
    // v0.2
    //   - Implemented anti-gravity movement (not working well)
    //
    class Robotron : AdvancedRobot
    {
        private readonly Double MinAngle = 10.0;
        private readonly Double MaxAngle = 90.0;

        private Double AngleRange = 0.0;
        private Double DoubleAngleRange = 0.0;

        private Dictionary<String, TrackedEnemy> trackedEnemies;
        private TrackedEnemy curTrackedEnemy = null;
        Vector2 AntigravityVector = new Vector2();

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

        //
        // Loops through all the tracked enemies and calculates useful values like the antigravity power
        // or who is closest to ourselves
        //
        private void ProcessEnemies()
        {
            AntigravityVector = new Vector2(0.0, 0.0);

            Double minDistance = Double.MaxValue;

            foreach (KeyValuePair<String, TrackedEnemy> pair in trackedEnemies)
            {
                pair.Value.UpdateFromPlayer(this);

                // Check distance
                if (pair.Value.Distance < minDistance)
                {
                    minDistance = pair.Value.Distance;

                    Out.WriteLine("Tracking enemy " + pair.Value.Name + " at distance " + minDistance);
                    curTrackedEnemy = pair.Value;
                }

                // Calculate antigravity
                AntigravityVector += pair.Value.AntigravityVector;
            }

            Out.WriteLine("Antigravity is " + AntigravityVector);
        }


        private Boolean MoveForward = true;
        private bool UseAntigravity = true;

        private void UpdateTank()
        {
            double AntigravityDistance = AntigravityVector.Module();

            Out.WriteLine("[AG] Antigravity distance is " + AntigravityDistance);

            // Not enough antigravity or there are walls
            if (!UseAntigravity || AntigravityDistance < 5 || X < 20 || X > (BattleFieldWidth - 20) || Y < 20 || Y < (BattleFieldHeight - 20))
            {
                if (MoveForward)
                {
                    if (DistanceRemaining == 0.0)
                    {
                        SetAhead(Util.GetRandom() * 100.0 + 100.0);
                        MoveForward = false;
                    }
                }
                else
                {
                    if (DistanceRemaining == 0.0)
                    {
                        SetBack(Util.GetRandom() * 100.0 + 100.0);
                        MoveForward = true;
                    }
                }
                SetTurnRight(360.0);
            }
            else
            {
                Out.WriteLine("[AG] Using antigravity!!");

                double antigravityAngleRadians = Util.CalculateBearingRadians(AntigravityVector.X, AntigravityVector.Y);
                double tankTurnRadians = Utils.NormalRelativeAngle(antigravityAngleRadians - HeadingRadians); // (-PI, PI)

                if (Math.Abs(tankTurnRadians) <= (Math.PI / 2))
                {
                    // Antigravity is on the forward direction
                    Out.WriteLine("[AG] Move ahead by " + AntigravityDistance);
                    SetAhead(AntigravityDistance);
                }
                else
                {
                    // Antigravity is on the backward direction
                    Out.WriteLine("[AG] Move back by " + AntigravityDistance);
                    SetBack(AntigravityDistance);
                }

                Out.WriteLine("[AG] Rotate by " + AntigravityDistance);
                SetTurnRight(tankTurnRadians);
            }
        }

        //
        // Gun methods
        //
        private void PointGunAt(TrackedEnemy curTrackedEnemy)
        {
            double enemyBearingRadians = Util.CalculateBearingRadians(X, Y, curTrackedEnemy.Position.X, curTrackedEnemy.Position.Y);
            double gunRotationRadians = Utils.NormalRelativeAngle(enemyBearingRadians - GunHeadingRadians);

            Out.WriteLine("Gun heading " + GunHeading);
            Out.WriteLine("Enemy relative bearing " + Utils.ToDegrees(enemyBearingRadians));

            Out.WriteLine("Rotating gun by " + gunRotationRadians + " radians (" + Utils.ToDegrees(gunRotationRadians) + " degrees)");
            SetTurnGunRightRadians(gunRotationRadians);
        }

        private void UpdateGun()
        {


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
            double enemyDistance = Util.CalculateDistance(X, Y, enemy.Position.X, enemy.Position.Y);

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
                ProcessEnemies();

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