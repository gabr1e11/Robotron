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
    // ROBOTRON
    //    'A robot for Picanhas competition'
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
    //    - Ramming for finishing robots
    // v0.6
    //    - Avoid walls when rotating around an enemy
    //    - Approach enemy again if it is too far away
    // v0.7
    //    - Team members go to their quadrants at the beginning
    //    - Team members notify each other of scanned robots
    // v0.8
    //    - Added tracking score formula
    //    - Added strategy configuration
    //
    public class Robotron : TeamRobot
    {
        [Flags]
        public enum EventFlags : short
        {
            HitWall = 0x01,
            BumpedEnemy = 0x02
        }

        public Behaviour.BehaviourStateMachine BehaviourStateMachine;
        public TrackedEnemies TrackedEnemies;
        public EventFlags Events { get; private set; }
        public bool IsTeamLeader = false;

        // INIT
        private void Init()
        {
            SetColors(Color.Red, Color.OrangeRed, Color.LightGoldenrodYellow);

            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            IsAdjustRadarForRobotTurn = true;

            TrackedEnemies = new TrackedEnemies(this);
            BehaviourStateMachine = new Behaviour.BehaviourStateMachine(this, new Behaviour.GoToQuadrantState(this));

            IsTeamLeader = (Util.GetTeamBotNumber(Name) == 1);

            ConfigureStrategy();
        }

        private void ConfigureStrategy()
        {
            Config config = new Config();

            // Minimum distance change to choose another enemy that is closer
            config.MinDistanceChange = 0.0;

            // Maximum number of turns to consider for danger score
            config.MaxBulletHitTimeDiff = 16 * 4;

            // Minimum energy an enemy has to have for us to ram into it
            config.MinEnergyForRamming = 40.0;

            // Minimum distance for full energy bullet
            config.MinDistanceHighEnergyBullet = 50.0;
            config.MaxDistanceLowEnergyBullet = 500.0;

            // Weights for the tracking score formula
            config.TrackingScoreDistanceWeight = 1.0;
            config.TrackingScoreDangerWeight = 0.0;
            config.TrackingScoreEnergyWeight = 0.0;

            // Team rules
            config.InitPosAllowedDistance = 50.0f;

            Strategy.SetConfig(config);
        }

        // MAIN LOOP
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
        public void SetFlag(EventFlags evt)
        {
            Events |= evt;
        }

        public bool IsFlagSet(EventFlags evt)
        {
            return (Events & evt) != 0x0;
        }

        public void ClearFlags()
        {
            Events = 0x0;
        }

        public void ClearFlag(EventFlags evt)
        {
            Events &= ~evt;
        }

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

            if (clockwise)
            {
                SetAhead(frontBackMultiplier * speed);
            }
            else
            {
                SetBack(frontBackMultiplier * speed);
            }

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
        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            base.OnScannedRobot(evnt);

            Enemy scannedEnemy = new Enemy(this, evnt);

            TrackedEnemies.OnScannedRobot(scannedEnemy);
            BroadcastMessage(new TeamEvent(TeamEventType.OnScannedRobot, scannedEnemy));
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
            SetFlag(EventFlags.HitWall);
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
            SetFlag(EventFlags.BumpedEnemy);
        }

        public override void OnSkippedTurn(SkippedTurnEvent evnt)
        {
            base.OnSkippedTurn(evnt);

            Log("======================[SKIPPED TURN]===========================");
        }

        // TEAM METHODS
        public override void OnMessageReceived(MessageEvent evnt)
        {
            base.OnMessageReceived(evnt);

            if (evnt.Message is TeamEvent)
            {
                TeamEvent teamEvent = (TeamEvent)evnt.Message;

                switch (teamEvent.Type)
                {
                    case TeamEventType.OnScannedRobot:
                        TrackedEnemies.OnScannedRobot(teamEvent.Enemy);
                        break;
                }
            }
        }
    }
}