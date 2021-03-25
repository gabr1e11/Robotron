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
    // v0.9
    //    - Notify other members of currently tracked enemy and
    //      when we stop tracking it to avoid tracking the same
    // v1.0
    //    - Added radar locking when tracking an enemy
    // v1.1
    //    - Tweaked radar locking when tracking an enemy
    //    - Refactored the state machines to make them more independant
    // v1.2
    //    - Added predictive enemy tracking
    // v1.3
    //    - Added win dance
    //    - Config for safe distance
    //    - Fixed a bug in radar locking
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
            SetColors(Color.LawnGreen, Color.DarkOliveGreen, Color.ForestGreen, Color.DarkSeaGreen, Color.GreenYellow);

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

            // SAFE DISTANCE
            //   Multipliers based on the robot energy
            config.SafeDistanceLowEnergyThreshold = 30.0;
            config.SafeDistanceMultiplierLowEnergy = 5.0;
            config.SafeDistanceMultiplierNormalEnergy = 3.0;
            //   Multipliers based on the enemy distance
            config.EnemyTooFarMultiplier = 2.0;
            config.EnemyTooCloseMultiplier = 0.5;
            config.EnemyCloseEnoughMaxMultiplier = 1.5;
            config.EnemyCloseEnoughMinMultiplier = 0.8;

            // BULLET ENERGY
            config.MinDistanceHighEnergyBullet = 50.0;
            config.MaxDistanceLowEnergyBullet = 500.0;

            // TRACKING SCORE (for selecting enemies)
            config.TrackingScoreDistanceWeight = 1.0;
            config.TrackingScoreDangerWeight = 0.0;
            config.TrackingScoreEnergyWeight = 0.0;
            //    Maximum number of turns to consider for danger score
            config.MaxBulletHitTimeDiff = 16 * 4;

            // TEAM QUADRANT
            config.InitPosAllowedDistance = 50.0f;

            // ENEMY LOCKED
            //    Angle of the radar swiping area
            config.RadarScanAreaSwipeAngleDegrees = 1.0;
            //    Maximum number of turns that the radar will lock onto the enemy
            config.LockRadarFocusMaxTurns = 20;
            //    Angular speed for rotation around an enemy
            config.RotationAroundEnemySpeedDegreesSec = 10.0;
            //    Enables/disables prediction of next enemy position
            config.EnableEnemyPositionPrediction = true;
            // Minimum energy an enemy has to have for us to ram into it
            config.MinEnergyForRamming = 40.0;

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

        public void PointGunAt(TrackedEnemy enemy)
        {
            double enemyBearingRadians = Util.CalculateBearingRadians(X, Y, enemy.Position.X, enemy.Position.Y);
            double gunRotationRadians = Utils.NormalRelativeAngle(enemyBearingRadians - GunHeadingRadians);

            Log("Gun heading " + GunHeading);
            Log("Enemy relative bearing " + Utils.ToDegrees(enemyBearingRadians));

            Log("Rotating gun by " + gunRotationRadians + " radians (" + Utils.ToDegrees(gunRotationRadians) + " degrees)");
            SetTurnGunRightRadians(gunRotationRadians);
        }

        public void PointRadarAt(TrackedEnemy enemy, Double offsetRadians)
        {
            double enemyBearingRadians = Util.CalculateBearingRadians(X, Y, enemy.Position.X, enemy.Position.Y);
            double radarRotationRadians = Utils.NormalRelativeAngle(enemyBearingRadians - RadarHeadingRadians);

            if (radarRotationRadians <= 0)
            {
                SetTurnRadarRightRadians(radarRotationRadians - offsetRadians);
            }
            else
            {
                SetTurnRadarRightRadians(radarRotationRadians + offsetRadians);
            }
        }

        // EVENTS
        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            base.OnScannedRobot(evnt);

            Enemy scannedEnemy = new Enemy(this, evnt);

            TrackedEnemies.OnScannedRobot(scannedEnemy);
            BroadcastMessage(new TeamEvent(TeamEventType.EnemyScanned, scannedEnemy));
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

        public override void OnWin(WinEvent evnt)
        {
            base.OnWin(evnt);

            BehaviourStateMachine.ChangeState(new Behaviour.NoopState());

            TurnLeft(Utils.NormalRelativeAngleDegrees(Heading - 90));
            TurnLeft(10.0);

            SetAllColors(Color.Green);
            TurnRight(20.0);
            SetAllColors(Color.DarkGreen);
            TurnLeft(20.0);
            SetAllColors(Color.DarkOliveGreen);
            TurnRight(20.0);
            SetAllColors(Color.DarkSeaGreen);
            TurnLeft(20.0);
            SetAllColors(Color.ForestGreen);
            TurnRight(20.0);
            SetAllColors(Color.GreenYellow);
            TurnLeft(20.0);
            SetAllColors(Color.LawnGreen);
            TurnLeft(20.0);
            SetAllColors(Color.LightGreen);
            TurnRight(20.0);
            SetAllColors(Color.LightSeaGreen);
            TurnLeft(20.0);
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
                    case TeamEventType.EnemyScanned:
                        TrackedEnemies.OnScannedRobot(teamEvent.Enemy);
                        break;
                    case TeamEventType.TrackingEnemy:
                        if (!IsTeamLeader)
                        {
                            TrackedEnemies.ClearBlacklistedEnemies();
                            if (Others > 1)
                            {
                                TrackedEnemies.AddBlacklistedEnemy(teamEvent.Enemy);
                            }
                        }
                        break;
                    case TeamEventType.SearchingForEnemy:
                        if (!IsTeamLeader)
                        {
                            TrackedEnemies.ClearBlacklistedEnemies();
                        }
                        break;
                }
            }
        }

        public void SendTeamEvent(TeamEventType type, Enemy enemy)
        {
            BroadcastMessage(new TeamEvent(type, enemy));
        }
    }
}