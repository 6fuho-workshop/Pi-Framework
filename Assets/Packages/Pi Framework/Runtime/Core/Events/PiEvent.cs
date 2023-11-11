
using PiFramework.Mediator;
using System;


namespace PiFramework
{
    public interface IPiEvent
    {
        /// <summary>
        /// Register callback
        /// </summary>
        IUnRegister Register(Action callback);

        /// <summary>
        /// Remove callback
        /// </summary>
        /// <param name="callback"></param>
        void UnRegister(Action callback);

        /// <summary>
        /// Add event callback nhưng ignore nếu callback đã được Add.<br/>
        /// Nếu bỏ qua thì IUnRegister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>IUnRegister Instruction to remove callback</returns>
        IUnRegister RegisterIfNotExists(Action callback);
    }

    public abstract class PiEventBase : IPiEvent
    {
        protected Action actions;

        /// <summary>
        /// Indicates whether the event is being dispatched. <br/> 
        /// An event cannot be redispatched while it being dispatched. 
        /// </summary>
        private bool invoking;

        public IUnRegister Register(Action callback)
        {
            actions += callback;
            return new CustomUnRegister(() => { UnRegister(callback); });
        }

        public IUnRegister RegisterIfNotExists(Action callback)
        {
            var unbinder = new CustomUnRegister(() => { UnRegister(callback); });
            if (Contains(actions, callback))
                unbinder.isEmpty = true;
            else
                actions += callback;
            return unbinder;
        }

        public void UnRegister(Action callback) => actions -= callback;

        public abstract void UnRegisterAll();

        protected bool CheckInvokable()
        {
            if (invoking)
            {
                UnityEngine.Debug.LogError("An event cannot be reinvoked while it being invoked.");
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

        public IUnRegister Register(Action<T> callback)
        {
            calls += callback;
            return new CustomUnRegister(() => { UnRegister(callback); });
        }

        /// <summary>
        /// Add event listener nhưng sẽ bỏ qua nếu listener đã được Add.<br/>
        /// Nếu bỏ qua thì IUnRegister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>Instruction to remove listenter</returns>
        public IUnRegister RegisterIfNotExists(Action<T> callback)
        {
            var unbinder = new CustomUnRegister(() => { UnRegister(callback); });
            if (PiEvent.Contains(calls, callback))
                unbinder.isEmpty = true;
            else
                calls += callback;
            return unbinder;
        }

        public void UnRegister(Action<T> callback) => calls -= callback;

        public override void UnRegisterAll()
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

        public override void UnRegisterAll() => actions = null;
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

        public IUnRegister Register(Action<T, K> callback)
        {
            calls += callback;
            return new CustomUnRegister(() => { UnRegister(callback); });
        }

        /// <summary>
        /// Add event listener nhưng sẽ bỏ qua nếu listener đã được Add.<br/>
        /// Nếu bỏ qua thì IUnRegister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>Instruction to remove listenter</returns>
        public IUnRegister RegisterIfNotExists(Action<T, K> callback)
        {
            var unbinder = new CustomUnRegister(() => { UnRegister(callback); });
            if (PiEvent.Contains(calls, callback))
                unbinder.isEmpty = true;
            else
                calls += callback;
            return unbinder;
        }

        public void UnRegister(Action<T, K> callback) => calls -= callback;

        public void Invoke(T t, K k)
        {
            if (CheckInvokable())
            {
                actions?.Invoke();
                calls?.Invoke(t, k);
                PostInvoke();
            }
        }

        public override void UnRegisterAll()
        {
            actions = null;
            calls = null;
        }
    }

    public class PiEvent<T, K, S> : PiEventBase
    {
        private Action<T, K, S> calls;

        public IUnRegister Register(Action<T, K, S> callback)
        {
            calls += callback;
            return new CustomUnRegister(() => { UnRegister(callback); });
        }

        /// <summary>
        /// Add event listener nhưng sẽ bỏ qua nếu listener đã được Add.<br/>
        /// Nếu bỏ qua thì IUnRegister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>Instruction to remove listenter</returns>
        public IUnRegister RegisterIfNotExists(Action<T, K, S> callback)
        {
            var unbinder = new CustomUnRegister(() => { UnRegister(callback); });
            if (PiEvent.Contains(calls, callback))
                unbinder.isEmpty = true;
            else
                calls += callback;
            return unbinder;
        }

        public void UnRegister(Action<T, K, S> callback) => calls -= callback;

        public void Invoke(T t, K k, S s)
        {
            if (CheckInvokable())
            {
                actions?.Invoke();
                calls?.Invoke(t, k, s);
                PostInvoke();
            }
        }

        public override void UnRegisterAll()
        {
            actions = null;
            calls = null;
        }
    }
}