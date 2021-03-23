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
    // Robotron v0.3
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
    // v0.3
    //    - Added rotation around the enemy when attacking
    // v0.4
    //    - Tried to implement tracking enemies with a distance threshold
    //      but it didn't improve
    // v0.5
    //    - Implemented ramming for finishing robots
    //
    public class Robotron : TeamRobot
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
            BehaviourStateMachine = new Behaviour.BehaviourStateMachine(this, new Behaviour.WaitForTrackedEnemyState(this));
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
        public void GoToPosition(Vector2 position)
        {
            Vector2 targetVector = new Vector2(position.X - X, position.Y - Y);

            double angleToTargetRadians = Util.CalculateBearingRadians(X, Y, position.X, position.Y);
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

        public void RotateAroundPosition(Vector2 position, Double angularSpeedRadians, bool clockwise)
        {
            Vector2 targetVector = new Vector2(position.X - X, position.Y - Y);

            double angleToTargetRadians = Util.CalculateBearingRadians(X, Y, position.X, position.Y);
            double targetAngleRadians = Utils.NormalRelativeAngle(angleToTargetRadians - HeadingRadians); // (-PI, PI)

            double distance = targetVector.Module();
            Double speed = angularSpeedRadians * distance;

            double turnAngle = System.Math.Atan(System.Math.Tan(targetAngleRadians));

            Double frontBackMultiplier = 1.0;
            if (targetAngleRadians == turnAngle)
            {
                if (turnAngle >= 0.0)
                {
                    turnAngle -= System.Math.PI / 2.0;
                    frontBackMultiplier = -1.0;
                }
                else
                {
                    turnAngle += System.Math.PI / 2.0;
                    frontBackMultiplier = 1.0;
                }
            }
            else
            {
                if (turnAngle >= 0.0)
                {
                    turnAngle -= System.Math.PI / 2.0;
                    frontBackMultiplier = 1.0;
                }
                else
                {
                    turnAngle += System.Math.PI / 2.0;
                    frontBackMultiplier = -1.0;
                }
            }

            if (speed > 3 * Rules.MAX_VELOCITY)
            {
                Double rate = 3 * Rules.MAX_VELOCITY / speed;

                turnAngle *= rate;

                speed = 3 * Rules.MAX_VELOCITY;
            }

            SetAhead(frontBackMultiplier * speed);
            SetTurnRightRadians(turnAngle * 3.0);

            Log("Current Speed=" + Velocity);
            Log("Around point=" + position + ", Robot position=" + new Vector2(X, Y));
            Log("distance=" + distance + ", speed=" + speed + ", turnAngle=" + Utils.ToDegrees(turnAngle));

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
            base.OnScannedRobot(enemy);

            TrackedEnemies.OnScannedRobot(enemy);
        }

        public override void OnRobotDeath(RobotDeathEvent enemy)
        {
            base.OnRobotDeath(enemy);

            TrackedEnemies.OnRobotDeath(enemy);
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            base.OnHitWall(evnt);

            Log("HIT A WALL");
        }

        public override void OnBulletHit(BulletHitEvent evnt)
        {

        }

        public override void OnBulletHitBullet(BulletHitBulletEvent evnt)
        {

        }

        public override void OnBulletMissed(BulletMissedEvent evnt)
        {

        }

        public override void OnHitByBullet(HitByBulletEvent evnt)
        {
            TrackedEnemies.OnHitByBullet(evnt);
        }

        public override void OnHitRobot(HitRobotEvent evnt)
        {

        }

        public override void OnSkippedTurn(SkippedTurnEvent evnt)
        {
            base.OnSkippedTurn(evnt);

            Log("======================[SKIPPED TURN]===========================");
        }
    }
}