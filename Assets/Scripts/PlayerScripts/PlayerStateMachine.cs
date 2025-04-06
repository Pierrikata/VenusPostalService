using System;
using System.Collections.Generic;
using StateMachineScripts;

namespace PlayerScripts
{
    public class PlayerStateMachine : IStateMachine
    {
        #region Constructor

        private readonly Player _player;
        public PlayerStateMachine(Player player) { _player = player; }
        
        #endregion
        #region Current & Previous States

        internal BaseState CurrentState, PreviousState;
        BaseState IStateMachine.Current
        {
            get => CurrentState;
            set => CurrentState = value;
        }
        BaseState IStateMachine.Previous
        {
            get => PreviousState;
            set => PreviousState = value;
        }
        
        #endregion
        #region StateCache

        private static readonly Dictionary<Type, BaseState> PlayerStates = new Dictionary<Type, BaseState>();
        public void AddState(BaseState state)
        {
            var type = state.GetType();
            PlayerStates.TryAdd(type, state);
        }
        public T GetState<T>() where T : BaseState
        {
            var type = typeof(T);

            if (PlayerStates.TryGetValue(type, out var state))
                return state as T;
            return null;
        }

        #endregion
        #region RuntimeMethods
        
        public void Awake()
        {
            // TODO: write concrete states
            AddState(new PlayerGroundedState(_player, this));
            AddState(new Jump(_player, this));
            AddState(new FreeFall(_player, this));
            AddState(new WallSlide(_player, this));
            AddState(new WallJump(_player, this));
        }
        public void Initializer(BaseState startState)
        {
            CurrentState = startState;
            CurrentState.Enter();
        }
        public void Update() { CurrentState?.Update(); }
        public void FixedUpdate() { CurrentState?.FixedUpdate(); }
        public void ChangeState(BaseState newState)
        {
            CurrentState.Exit();
            PreviousState = CurrentState;
            CurrentState = newState;
            CurrentState.Enter();
        }

        #endregion
    }
}
