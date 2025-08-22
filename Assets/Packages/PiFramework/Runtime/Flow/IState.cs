using System;
using UnityEngine;

namespace PF.FlowNode
{
    //An interface for states. Works together with IStateCallbackReceiver
    public interface IState
    {
        ///<summary>The name of the state</summary>
        //string Name { get; }
        ///<summary>The tag of the state</summary>
        //string Tag { get; }
        ///<summary>The elapsed time of the state</summary>
        //float ElapsedTime { get; }
        ///<summary>The FSM this state belongs to</summary>
        //FSM FSM { get; }
        ///<summary>An array of the state's transition connections</summary>
        //FSMConnection[] GetTransitions();
        ///<summary>Evaluates the state's transitions and returns true if a transition has been performed</summary>
        //bool CheckTransitions();
        ///<summary>Marks the state as Finished</summary>
        //void Finish(bool success);

        bool Condition();
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
    }

    public class FSMState : FSMNode
    {
        public override string NodeName => "FSM State";
        public override string NodeDescription => "Represents a state within an FSM Graph.";

        public override void Initialize()
        {
            // Initialization logic for FSMState
        }
    }

    /// <summary>
    /// A flexible implementation of the IState interface that allows
    /// custom behavior to be injected via delegates for each state method.
    /// Useful for quickly defining states without creating new classes.
    /// </summary>
    public class CustomState : IState
    {
        private Func<bool> mOnCondition;
        private Action mOnEnter;
        private Action mOnUpdate;
        private Action mOnFixedUpdate;
        private Action mOnExit;

        public CustomState OnCondition(Func<bool> onCondition)
        {
            mOnCondition = onCondition;
            return this;
        }

        public CustomState OnEnter(Action onEnter) { mOnEnter = onEnter; return this; }
        public CustomState OnUpdate(Action onUpdate) { mOnUpdate = onUpdate; return this; }
        public CustomState OnFixedUpdate(Action onFixedUpdate) { mOnFixedUpdate = onFixedUpdate; return this; }
        public CustomState OnExit(Action onExit) { mOnExit = onExit; return this; }

        public bool Condition() { return mOnCondition?.Invoke() ?? true; }
        public void Enter() => mOnEnter?.Invoke();
        public void Update() => mOnUpdate?.Invoke();
        public void FixedUpdate() => mOnFixedUpdate?.Invoke();
        public void Exit() => mOnExit?.Invoke();
    }

    /// <summary> An abstract base class for states that can be used in an FSM.
    /// TStateId có thể là string, int, enum, hoặc bất kỳ kiểu dữ liệu nào khác mà bạn muốn sử dụng để định danh.
    /// TTarget có thể là bất cứ gì muốn liên kết với State tùy cách thiết kế concrete, 
    /// chẳng hạn như một GameObject, một component, hoặc chính mono điều khiển FSM.</summary>
    public abstract class AbstractState<TStateId, TTarget> : IState
    {
        protected BasicFSM<TStateId> FSM;
        protected TTarget Target;

        public AbstractState(BasicFSM<TStateId> fsm, TTarget target)
        {
            FSM = fsm;
            Target = target;
        }

        bool IState.Condition() => OnCondition();
        void IState.Enter() => OnEnter();
        void IState.Update() => OnUpdate();
        void IState.FixedUpdate() => OnFixedUpdate();
        void IState.Exit() => OnExit();

        protected virtual bool OnCondition() => true;
        protected virtual void OnEnter() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnExit() { }
    }
}