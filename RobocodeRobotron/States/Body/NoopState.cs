using System;

namespace RC.Body
{
    public class NoopState : Body.State
    {
        public void Enter(Robotron entity)
        {
        }

        public void Execute(Robotron entity)
        {
        }

        public void Exit(Robotron entity)
        {
        }
    }
}