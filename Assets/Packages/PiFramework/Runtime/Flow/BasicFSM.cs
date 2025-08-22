using System;
using System.Collections.Generic;
using UnityEngine;

namespace PF.FlowNode
{
    /// <summary>
    /// A generic Finite State Machine (FSM) implementation for managing state transitions.
    /// Allows adding, retrieving, and switching between states of type <typeparamref name="TStateID"/>.
    /// Supports custom state logic via the IState interface and notifies on state changes.
    /// </summary>
    /// <typeparam name="TStateID">The type used to identify states (e.g., enum, string).</typeparam>
    public class BasicFSM<TStateID>
    {
        protected Dictionary<TStateID, IState> mStates = new();

        public bool StrictMode { get; set; } = true; // true = throw, false = log warning
        private bool _autoInitWarned;

        public virtual BasicFSM<TStateID> AddState(TStateID id, IState state) { mStates.Add(id, state); return this; }

        public virtual CustomState CustomState(TStateID key)
        {
            if (mStates.ContainsKey(key))
                return mStates[key] as CustomState;

            var state = new CustomState();
            mStates.Add(key, state);
            return state;
        }

        public IState CurrentState { get; private set; }
        public TStateID CurrentStateId { get; private set; }
        public TStateID PreviousStateId { get; private set; }

        public long FrameCountOfCurrentState = 1;

        public virtual void ChangeState(TStateID stateId, bool force = false)
        {
            // If the state is already the current state, do nothing
            if (stateId.Equals(CurrentStateId))
                return;

            if (!TryGetState(stateId, out var state))
                return;

            if (CurrentState == null)
            {
                if (!_autoInitWarned)
                {
                    Debug.LogWarning($"FSM: Auto-initializing with state '{stateId}'. Consider calling Initialize() explicitly for clarity.");
                    _autoInitWarned = true;
                }
                if (force || state.Condition())
                    Initialize(stateId);
                else
                    Debug.LogWarning($"FSM: Auto-init skipped because condition for state '{stateId}' failed.");
                return;
            }

            if (force || state.Condition())
            {
                CurrentState.Exit();
                PreviousStateId = CurrentStateId;
                CurrentState = state;
                CurrentStateId = stateId;
                mOnStateChanged?.Invoke(PreviousStateId, CurrentStateId);
                FrameCountOfCurrentState = 1;
                CurrentState.Enter();
            }
        }

        protected bool TryGetState(TStateID id, out IState state)
        {
            if (!mStates.TryGetValue(id, out state))
            {
                if (StrictMode)
                    throw new KeyNotFoundException($"FSM: State '{id}' not found.");
                else
                    Debug.LogWarning($"FSM: State '{id}' not found. Ignored.");
                return false;
            }
            return true;
        }

        private Action<TStateID, TStateID> mOnStateChanged = (_, __) => { };

        public virtual void OnStateChanged(Action<TStateID, TStateID> onStateChanged)
        {
            mOnStateChanged += onStateChanged;
        }

        public virtual void Initialize(TStateID startState)
        {
            if (!TryGetState(startState, out var state))
                return;
 
            PreviousStateId = startState;
            CurrentState = state;
            CurrentStateId = startState;
            FrameCountOfCurrentState = 0;
            state.Enter();
        }

        public virtual void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }

        public virtual void Tick()
        {
            CurrentState?.Update();
            FrameCountOfCurrentState++;
        }

        public virtual void Clear()
        {
            CurrentState?.Exit();
            CurrentState = null;
            CurrentStateId = default;
            mStates.Clear();
        }
    }

    [CreateAssetMenu(fileName = "FSMTransitions", menuName = "▶ Pi ◀/FSM Transitions")]
    public class FSMTransitionAsset : ScriptableObject
    {
        [Serializable]
        public struct TransitionData
        {
            public string From;
            public string To;
            public string ConditionKey; // dùng để map tới Func<bool> runtime
        }

        public List<TransitionData> Transitions = new();
    }


    public abstract class AbstractGraphFSM<TStateID> : BasicFSM<TStateID>
    {
        protected sealed class Transition
        {
            public TStateID From;
            public TStateID To;
            public Func<bool> Condition;
        }

        protected List<Transition> Transitions { get; } = new();

        /// <summary>
        /// Subclass phải override để parse ID từ string trong asset sang TStateID.
        /// </summary>
        protected abstract TStateID ParseStateId(string rawId);

        public AbstractGraphFSM<TStateID> AddTransition(TStateID from, TStateID to, Func<bool> condition = null)
        {
            Transitions.Add(new Transition
            {
                From = from,
                To = to,
                Condition = condition ?? (() => true)
            });
            return this;
        }

        public AbstractGraphFSM<TStateID> LoadTransitions( FSMTransitionAsset asset,
            Dictionary<string, Func<bool>> conditionMap)
        {
            foreach (var t in asset.Transitions)
            {
                var fromId = ParseStateId(t.From);
                var toId = ParseStateId(t.To);

                if (!conditionMap.TryGetValue(t.ConditionKey, out var cond))
                {
                    Debug.LogWarning($"Condition '{t.ConditionKey}' not found. Defaulting to always true.");
                    cond = () => true;
                }

                AddTransition(fromId, toId, cond);
            }
            return this;
        }

        public void ClearTransitions()
        {
            Transitions.Clear();
        }

        private void CheckTransitions()
        {
            foreach (var t in Transitions)
            {
                if (EqualityComparer<TStateID>.Default.Equals(t.From, CurrentStateId) && t.Condition())
                {
                    ChangeState(t.To);
                    break;
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (CurrentState != null)
                CheckTransitions();
        }
    }
}