using System;
using PF.Contracts;

namespace PF.Events
{
    // Generic base for all event types using delegate
    public abstract class PiEventBase<TDelegate> : IDisposable, IEvent where TDelegate : Delegate
    {
        protected TDelegate typedActions;
        protected Action actions;

        private bool invoking;
        private readonly object _lock = new object();

        // Register for typed delegate
        public IUnregister Register(TDelegate callback)
        {
            lock (_lock)
            {
                typedActions = Delegate.Combine(typedActions, callback) as TDelegate;
            }
            return new CustomUnregister(() => Unregister(callback));
        }

        public void Unregister(TDelegate callback)
        {
            lock (_lock)
            {
                typedActions = Delegate.Remove(typedActions, callback) as TDelegate;
            }
        }

        // Register for Action (no args)
        public IUnregister Register(Action callback)
        {
            lock (_lock)
            {
                actions += callback;
            }
            return new CustomUnregister(() => Unregister(callback));
        }

        public void Unregister(Action callback)
        {
            lock (_lock)
            {
                actions -= callback;
            }
        }

        public virtual void Dispose()
        {
            lock (_lock)
            {
                actions = null;
                typedActions = null;
            }
        }

        protected bool CheckInvokable()
        {
            lock (_lock)
            {
                if (invoking) return false;
                invoking = true;
                return true;
            }
        }
        protected virtual void PostInvoke()
        {
            lock (_lock)
            {
                invoking = false;
            }
        }

        // Helper to safely copy delegates for invocation
        protected (Action actionsCopy, TDelegate typedActionsCopy) GetActionsSnapshot()
        {
            lock (_lock)
            {
                return (actions, typedActions);
            }
        }
    }

    // No-argument event
    public class PiEvent : PiEventBase<Action>, IEventPublisher
    {
        public void Publish()
        {
            if (CheckInvokable())
            {
                var (actionsCopy, typedCopy) = GetActionsSnapshot();
                try
                {
                    actionsCopy?.Invoke();
                    typedCopy?.Invoke();
                }
                finally
                {
                    PostInvoke();
                }
            }
        }
    }

    // One-argument event
    public class PiEvent<T> : PiEventBase<Action<T>>, IEvent<T>, IEventPublisher<T>
    {
        public void Publish(T t)
        {
            if (CheckInvokable())
            {
                var (actionsCopy, typedCopy) = GetActionsSnapshot();
                try
                {
                    actionsCopy?.Invoke();
                    typedCopy?.Invoke(t);
                }
                finally
                {
                    PostInvoke();
                }
            }
        }
    }

    // Two-argument event
    public class PiEvent<T, K> : PiEventBase<Action<T, K>>, IEvent<T, K>, IEventPublisher<T, K>
    {
        public void Publish(T t, K k)
        {
            if (CheckInvokable())
            {
                var (actionsCopy, typedCopy) = GetActionsSnapshot();
                try
                {
                    actionsCopy?.Invoke();
                    typedCopy?.Invoke(t, k);
                }
                finally
                {
                    PostInvoke();
                }
            }
        }
    }

    // Three-argument event
    public class PiEvent<T, K, S> : PiEventBase<Action<T, K, S>>, IEvent<T, K, S>, IEventPublisher<T, K, S>
    {
        public void Publish(T t, K k, S s)
        {
            if (CheckInvokable())
            {
                var (actionsCopy, typedCopy) = GetActionsSnapshot();
                try
                {
                    actionsCopy?.Invoke();
                    typedCopy?.Invoke(t, k, s);
                }
                finally
                {
                    PostInvoke();
                }
            }
        }
    }
}