using System;

using RC.FSM;

namespace RC.Behaviour
{
    public class BehaviourStateMachine
    {
        private FiniteStateMachine<BehaviourStateMachine> BehaviourFSM = null;
        private FiniteStateMachine<BehaviourStateMachine> BodyStateMachine = null;
        private FiniteStateMachine<BehaviourStateMachine> GunStateMachine = null;
        private FiniteStateMachine<BehaviourStateMachine> RadarStateMachine = null;

        public Robotron Robot = null;

        public BehaviourStateMachine(Robotron robot, Behaviour.State initState)
        {
            Robot = robot;

            BodyStateMachine = new FiniteStateMachine<BehaviourStateMachine>(this, null);
            GunStateMachine = new FiniteStateMachine<BehaviourStateMachine>(this, null);
            RadarStateMachine = new FiniteStateMachine<BehaviourStateMachine>(this, null);

            BehaviourFSM = new FiniteStateMachine<BehaviourStateMachine>(this, initState);
        }

        public void ChangeState(FSMState<BehaviourStateMachine> newState)
        {
            BehaviourFSM.ChangeState(newState);
        }

        public void ProcessState()
        {
            BehaviourFSM.ProcessState();

            BodyStateMachine.ProcessState();
            GunStateMachine.ProcessState();
            RadarStateMachine.ProcessState();
        }

        public void ChangeBodyState(Body.State newState)
        {
            BodyStateMachine.ChangeState(newState);
        }

        public void ChangeGunState(Gun.State newState)
        {
            GunStateMachine.ChangeState(newState);
        }

        public void ChangeRadarState(Radar.State newState)
        {
            RadarStateMachine.ChangeState(newState);
        }

    }

}