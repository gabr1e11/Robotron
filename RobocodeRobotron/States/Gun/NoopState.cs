using System;

namespace RC.Gun
{
    public class NoopState : Gun.State
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