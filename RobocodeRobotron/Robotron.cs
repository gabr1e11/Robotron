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
using RC.Math;

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
    public class Robotron : AdvancedRobot
    {
        public Behaviour.BehaviourStateMachine BehaviourStateMachine;
        public TrackedEnemies TrackedEnemies;

        //
        // Robocode init + main loop
        //
        private void Init()
        {
            SetColors(Color.Red, Color.OrangeRed, Color.LightGoldenrodYellow);

            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            IsAdjustRadarForRobotTurn = true;

            TrackedEnemies = new TrackedEnemies(this);
            BehaviourStateMachine = new Behaviour.BehaviourStateMachine(this);
        }

        public override void Run()
        {
            base.Run();

            Init();

            while (true)
            {
                TrackedEnemies.Update();
                BehaviourStateMachine.ProcessState();
                Execute();
            }
        }

        // CONTROL METHODS
        public void GoToXY(Vector2 newPosition)
        {
            Vector2 targetVector = new Vector2(newPosition.X - X, newPosition.Y - Y);

            double angleToTargetRadians = Util.CalculateBearingRadians(X, Y, newPosition.X, newPosition.Y);
            double targetAngleRadians = Utils.NormalRelativeAngle(angleToTargetRadians - HeadingRadians); // (-PI, PI)

            double distance = targetVector.Module();

            double turnAngle = System.Math.Atan(System.Math.Tan(targetAngleRadians));
            if (targetAngleRadians == turnAngle)
            {
                Log("Move ahead by " + distance);
                SetAhead(distance);
            }
            else
            {
                Log("Move back by " + distance);
                SetBack(distance);
            }

            Log("Rotate by " + Utils.ToDegrees(turnAngle));
            SetTurnRightRadians(turnAngle);
        }

        public void StopTank()
        {
            SetAhead(0.0f);
            SetTurnRight(0.0f);
        }

        public void PointGunAt(TrackedEnemy curTrackedEnemy)
        {
            double enemyBearingRadians = Util.CalculateBearingRadians(X, Y, curTrackedEnemy.Position.X, curTrackedEnemy.Position.Y);
            double gunRotationRadians = Utils.NormalRelativeAngle(enemyBearingRadians - GunHeadingRadians);

            Log("Gun heading " + GunHeading);
            Log("Enemy relative bearing " + Utils.ToDegrees(enemyBearingRadians));

            Log("Rotating gun by " + gunRotationRadians + " radians (" + Utils.ToDegrees(gunRotationRadians) + " degrees)");
            SetTurnGunRightRadians(gunRotationRadians);
        }

        // EVENTS
        public override void OnScannedRobot(ScannedRobotEvent enemy)
        {
            TrackedEnemies.OnScannedRobot(enemy);
        }

        public override void OnRobotDeath(RobotDeathEvent enemy)
        {
            TrackedEnemies.OnRobotDeath(enemy);
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            Log("HIT A WALL");
        }
    }
}