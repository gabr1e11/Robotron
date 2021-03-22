﻿using System;

using Robocode;

using RC.Math;
using static RC.Logger;

namespace RC.Body
{
    public class ApproachEnemyState : Body.State
    {
        private TrackedEnemy Enemy = null;

        public ApproachEnemyState(TrackedEnemy enemy)
        {
            Enemy = enemy;
        }

        public void Enter(Robotron robot)
        {

        }

        public void Execute(Robotron robot)
        {
            Vector2 robotXY = new Vector2(robot.X, robot.Y);
            Vector2 robotEnemyVectorNorm = (Enemy.Position - robotXY).GetNormalized();

            Double SafeDistanceThreshold = 3.0 * robot.Width + Physics.Constants.MaxTankMovementPerTurn;
            Double safeDistance = Enemy.Distance - SafeDistanceThreshold;

            Vector2 targetXY = robotXY + robotEnemyVectorNorm * safeDistance;

            robot.GoToPosition(targetXY);
        }

        public void Exit(Robotron robot)
        {

        }
    }
}