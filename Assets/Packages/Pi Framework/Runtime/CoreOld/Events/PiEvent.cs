
using PF.Mediator;
using System;
using PF.Core.Common;

namespace PF
{
    public interface IPiEvent
    {
        /// <summary>
        /// Register callback
        /// </summary>
        IUnregister Register(Action callback);

        /// <summary>
        /// Remove callback
        /// </summary>
        /// <param name="callback"></param>
        void Unregister(Action callback);

        /// <summary>
        /// Add event callback nhưng ignore nếu callback đã được Add.<br/>
        /// Nếu bỏ qua thì IUnregister return sẽ có thuộc tính IsRegistered = false.
        /// </summary>
        /// <returns>IUnregister Instruction to remove callback</returns>
        IUnregister RegisterIfNotExists(Action callback);
    }

    public abstract class PiEventBase : IPiEvent
    {
        protected Action actions;

        /// <summary>
        /// Indicates whether the event is being dispatched. <br/> 
        /// An event cannot be redispatched while it being dispatched. 
        /// </summary>
        private bool invoking;

        public IUnregister Register(Action callback)
        {
            actions += callback;
            return new CustomUnregister(() => { Unregister(callback); });
        }

        public IUnregister RegisterIfNotExists(Action callback)
        {
            var success = true;
            if (Contains(actions, callback))
                success = false;
            else
                actions += callback;
            return new CustomUnregister(() => { Unregister(callback); }, success);
        }

        public void Unregister(Action callback) => actions -= callback;

        public abstract void UnregisterAll();

        protected bool CheckInvokable()
        {
            if (invoking)
            {
                //UnityEngine.Debug.LogError("An event cannot be reinvoked while it being invoked.");
                return false;
            }
            else
            {
                invoking = true;
                return true;
            }
        }
        protected virtual void PostInvoke()
        {
            invoking = false;
        }

        /// <summary>
        /// Internal Helper.<br/> check if the invoker is in InvocationList of delegate d.
        /// </summary>
        internal static bool Contains(Delegate d, Delegate invoker)
        {
            if (d == null)
                return false;
            var arr = d.GetInvocationList();
            for (uint i = 0; i < arr.Length; ++i)
            {
                if (arr[i].Equals(invoker))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public abstract class PiEventBase<T> : PiEventBase
    {
        protected Action<T> calls;

        public IUnregister Register(Action<T> callback)
        {
            calls += callback;
            return new CustomUnregister(() => { Unregister(callback); });
        }

        /// <summary>
        /// Add event listener nhưng sẽ bỏ qua nếu listener đã được Add.<br/>
        /// Nếu bỏ qua thì IUnregister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>Instruction to remove listenter</returns>
        public IUnregister RegisterIfNotExists(Action<T> callback)
        {
            var success = true;
            if (PiEvent.Contains(calls, callback))
                success = false;
            else
                calls += callback;
            return new CustomUnregister(() => { Unregister(callback); } , success);
        }

        public void Unregister(Action<T> callback) => calls -= callback;

        public override void UnregisterAll()
        {
            actions = null;
            calls = null;
        }
    }

    public class PiEvent : PiEventBase
    {
        public void Invoke()
        {
            if (CheckInvokable())
            {
                actions?.Invoke();
                PostInvoke();
            }
        }

        public override void UnregisterAll() => actions = null;
    }

    public class PiEvent<T> : PiEventBase<T>
    {
        public void Invoke(T t)
        {
            if (CheckInvokable())
            {
                actions?.Invoke();
                calls?.Invoke(t);
                PostInvoke();
            }
        }
    }

    public class PiEvent<T, K> : PiEventBase
    {
        private Action<T, K> calls;

        public IUnregister Register(Action<T, K> callback)
        {
            calls += callback;
            return new CustomUnregister(() => { Unregister(callback); });
        }

        /// <summary>
        /// Add event listener nhưng sẽ bỏ qua nếu listener đã được Add.<br/>
        /// Nếu bỏ qua thì IUnregister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>Instruction to remove listenter</returns>
        public IUnregister RegisterIfNotExists(Action<T, K> callback)
        {
            var success = true;
            if (Contains(calls, callback))
                success = false;
            else
                calls += callback;
            return new CustomUnregister(() => { Unregister(callback); }, success);
        }

        public void Unregister(Action<T, K> callback) => calls -= callback;

        public void Invoke(T t, K k)
        {
            if (CheckInvokable())
            {
                actions?.Invoke();
                calls?.Invoke(t, k);
                PostInvoke();
            }
        }

        public override void UnregisterAll()
        {
            actions = null;
            calls = null;
        }
    }

    public class PiEvent<T, K, S> : PiEventBase
    {
        private Action<T, K, S> calls;

        public IUnregister Register(Action<T, K, S> callback)
        {
            calls += callback;
            return new CustomUnregister(() => { Unregister(callback); });
        }

        /// <summary>
        /// Add event listener nhưng sẽ bỏ qua nếu listener đã được Add.<br/>
        /// Nếu bỏ qua thì IUnregister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>Instruction to remove listenter</returns>
        public IUnregister RegisterIfNotExists(Action<T, K, S> callback)
        {
            var success = true;
            if (Contains(calls, callback))
                success = false;
            else
                calls += callback;
            return new CustomUnregister(() => { Unregister(callback); }, success);
        }

        public void Unregister(Action<T, K, S> callback) => calls -= callback;

        public void Invoke(T t, K k, S s)
        {
            if (CheckInvokable())
            {
                actions?.Invoke();
                calls?.Invoke(t, k, s);
                PostInvoke();
            }
        }

        public override void UnregisterAll()
        {
            actions = null;
            calls = null;
        }
    }
}