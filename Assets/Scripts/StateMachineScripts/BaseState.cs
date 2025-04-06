namespace StateMachineScripts
{
    public abstract class BaseState
    {
        public abstract void Enter();
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void Exit();
    }
}
