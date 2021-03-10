using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Numerics;
using System.Reflection;

using Robocode;
using Robocode.Util;

using static RC.Logger;

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

    // Robotron v0.2.2
    //
    // Robot for Picanhas competition
    //
    // v0.1
    //   - Created discovery radar (turns all the time)
    //   - Tracked all enemies
    //   - Point gun at closest enemy and fire
    // v0.2.1
    //   - Implemented anti-gravity movement (not working well)
    // v0.2.2
    //   - Fixed coordinate system to compass coordinate system
    //   - Added log system
    //   - Antigravity works better now
    //
    class Robotron : AdvancedRobot
    {
        private readonly Double MinAngle = 10.0;
        private readonly Double MaxAngle = 90.0;

        private Double AngleRange = 0.0;
        private Double DoubleAngleRange = 0.0;

        private Dictionary<String, TrackedEnemy> TrackedEnemies;
        private TrackedEnemy CurTrackedEnemy = null;
        Vector2 AntigravityVector = new Vector2();

        private void Init()
        {
            AngleRange = MaxAngle - MinAngle;
            DoubleAngleRange = 2 * AngleRange;

            SetColors(Color.Red, Color.OrangeRed, Color.LightGoldenrodYellow);

            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            IsAdjustRadarForRobotTurn = true;

            TrackedEnemies = new Dictionary<string, TrackedEnemy>(Others);
        }

        //
        // Loops through all the tracked enemies and calculates useful values like the antigravity power
        // or who is closest to ourselves
        //
        private void ProcessEnemies()
        {
            AntigravityVector = new Vector2(0.0, 0.0);

            Double minDistance = Double.MaxValue;

            foreach (KeyValuePair<String, TrackedEnemy> pair in TrackedEnemies)
            {
                pair.Value.UpdateFromPlayer(this);

                // Check distance
                if (pair.Value.Distance < minDistance)
                {
                    minDistance = pair.Value.Distance;

                    Log("Tracking enemy " + pair.Value.Name + " at distance " + minDistance);
                    CurTrackedEnemy = pair.Value;
                }

                // Calculate antigravity
                AntigravityVector += pair.Value.AntigravityVector;
            }

            Log("Antigravity is " + AntigravityVector);
        }

        bool IsPositionSafeFromWalls(Vector2 position)
        {
            return (position.X < (BattleFieldWidth - Width - 10)) && (position.X > Width + 10) &&
                   (position.Y < (BattleFieldHeight - Height - 10)) && (position.Y > Height + 10);
        }

        private void UpdateTank()
        {
            Vector2 antigravityNewPosition = CalculateXYFromDirection(AntigravityVector);
            double antigravityDistance = AntigravityVector.Module();

            Vector2 newPosition = new Vector2(X, Y);

            if (antigravityDistance >= 5.0)
            {
                Log("Using antigravity");
                newPosition = CalculateXYFromDirection(AntigravityVector);
            }
            else if (CurTrackedEnemy != null)
            {
                Log("Using enemy position: " + CurTrackedEnemy.Name);
                newPosition = CurTrackedEnemy.Position;
            }
            else
            {
                Log("Using random");

                // Just go somewhere
                double newX = X + Util.GetRandom() * 20.0 - 10.0;
                double newY = Y + Util.GetRandom() * 20.0 - 10.0;

                newPosition = new Vector2(newX, newY);
            }

            Log("Tank movement -> CurPos=" + new Vector2(X, Y) + " NewPos=" + newPosition);

            // Check for walls
            if (IsPositionSafeFromWalls(newPosition))
            {
                Log("Position is safe from walls");
                GoToXY(newPosition);
            }
            else
            {
                Log("Position is NOT safe from walls, stopping tank");
                StopTank();
            }

        }

        private void StopTank()
        {
            SetAhead(0.0f);
            SetTurnRight(0.0f);
        }

        //
        // Gun methods
        //
        private void PointGunAt(TrackedEnemy curTrackedEnemy)
        {
            double enemyBearingRadians = Util.CalculateBearingRadians(X, Y, curTrackedEnemy.Position.X, curTrackedEnemy.Position.Y);
            double gunRotationRadians = Utils.NormalRelativeAngle(enemyBearingRadians - GunHeadingRadians);

            Log("Gun heading " + GunHeading);
            Log("Enemy relative bearing " + Utils.ToDegrees(enemyBearingRadians));

            Log("Rotating gun by " + gunRotationRadians + " radians (" + Utils.ToDegrees(gunRotationRadians) + " degrees)");
            SetTurnGunRightRadians(gunRotationRadians);
        }

        private Vector2 CalculateXYFromDirection(Vector2 direction)
        {
            return (new Vector2(X, Y)) + direction;
        }

        private void GoToXY(Vector2 newPosition)
        {
            Vector2 tankPosition = new Vector2(X, Y);
            Vector2 enemyTankVector = newPosition - tankPosition;

            double angleRadians = Util.CalculateBearingRadians(enemyTankVector.X, enemyTankVector.Y);
            double tankTurnRadians = Utils.NormalRelativeAngle(angleRadians - HeadingRadians); // (-PI, PI)

            double distance = enemyTankVector.Module();

            if (Math.Abs(angleRadians - HeadingRadians) <= (Math.PI / 2))
            {
                Log("Move ahead by " + distance);
                SetAhead(distance);
            }
            else
            {
                Log("Move back by " + distance);
                SetBack(distance);
            }

            Log("Rotate by " + Utils.ToDegrees(tankTurnRadians));
            SetTurnRight(tankTurnRadians);
        }

        private void UpdateGun()
        {
            // If we are tracking an enemy, move the gun, then fire
            if (CurTrackedEnemy != null)
            {
                Log("Pointing gun at enemy " + CurTrackedEnemy.Name);
                PointGunAt(CurTrackedEnemy);

                if (Math.Abs(GunTurnRemaining) < 0.5)
                {
                    SmartFire(CurTrackedEnemy);
                }
            }
        }

        //
        // Radar methods
        //
        private void UpdateRadar()
        {
            // For now just rotate it at max speed
            Log("Rotating radar by 45 degrees");
            SetTurnRadarRight(45);
        }

        private void SmartFire(TrackedEnemy enemy)
        {
            double firePower = 0.1;
            double enemyDistance = Util.CalculateDistance(X, Y, enemy.Position.X, enemy.Position.Y);

            if (enemyDistance > 500.0)
            {
                firePower = 1;
            }
            else if (enemyDistance < 20.0)
            {
                firePower = 3.0;
            }
            else
            {
                firePower = 1.0 + 2.0 * (500.0 - enemyDistance) / 480.0;
            }

            Log("Firing Enemy: " + enemy.Name + " with power: " + firePower + "(Gun heat = " + GunHeat + ")");
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

            Log("Enemy " + enemy.Name + " detected at distance " + enemy.Distance);
            TrackedEnemies[enemy.Name] = new TrackedEnemy(this, enemy);
        }

        //
        // When another robot dies
        //
        public override void OnRobotDeath(RobotDeathEvent enemy)
        {
            base.OnRobotDeath(enemy);

            Log("Enemy " + enemy.Name + " died");
            if (CurTrackedEnemy.Name == enemy.Name)
            {
                CurTrackedEnemy = null;
            }

            TrackedEnemies.Remove(enemy.Name);
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            base.OnHitWall(evnt);

            Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Hit a wall");
        }
    }
}