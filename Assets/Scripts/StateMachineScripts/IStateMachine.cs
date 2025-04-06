using System;
using System.Collections.Generic;

namespace StateMachineScripts
{
    public interface IStateMachine
    {
        BaseState Current { get; protected internal set; }
        BaseState Previous { get; protected internal set; }

        #region StateCache
        
        void AddState(BaseState newState);
        T GetState<T>() where T : BaseState;
        
        #endregion
        #region RuntimeMethods
        
        void Awake();
        void Initializer(BaseState startState);
        void Update();
        void FixedUpdate();
        void ChangeState(BaseState newState);
        
        #endregion
    }
}
