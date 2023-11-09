
using PiFramework.Mediator;
using System;
using static UnityEditor.Progress;

namespace PiFramework
{
    public interface IPiEvent
    {
        /// <summary>
        /// Add a Action callback to PiEvent
        /// </summary>
        IUnRegister Register(Action callback);

        void UnRegister(Action callback);

        /// <summary>
        /// Add event listener nhưng sẽ bỏ qua nếu listener đã được Add.<br/>
        /// Nếu bỏ qua thì IUnRegister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>IUnRegister Instruction to remove listenter</returns>
        IUnRegister RegisterIfNotExists(Action callback);

        /// <summary>
        /// Remove all listeners
        /// </summary>
        
    }

    public abstract class PiEventBase : IPiEvent
    {
        protected Action actions;

        public IUnRegister Register(Action call)
        {
            actions += call;
            return new Unregister(() => { UnRegister(call); });
        }

        public IUnRegister RegisterIfNotExists(Action call)
        {
            var unbinder = new Unregister(() => { UnRegister(call); });
            if (Contains(actions, call))
                unbinder.isEmpty = true;
            else
                actions += call;
            return unbinder;
        }

        public void UnRegister(Action call) => actions -= call;

        public abstract void UnRegisterAll();

        /// <summary>
        /// Internal Helper.<br/> check if the invoker is in InvocationList of deledate d.
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

    public class PiEvent : PiEventBase
    {
        public void Invoke() => actions?.Invoke();

        public override void UnRegisterAll() => actions = null;
    }

    public class PiEvent<T> : PiEventBase
    {
        private Action<T> calls;

        public IUnRegister Register(Action<T> call)
        {
            calls += call;
            return new Unregister(() => { UnRegister(call); });
        }

        /// <summary>
        /// Add event listener nhưng sẽ bỏ qua nếu listener đã được Add.<br/>
        /// Nếu bỏ qua thì IUnRegister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>Instruction to remove listenter</returns>
        public IUnRegister RegisterIfNotExists(Action<T> call)
        {
            var unbinder = new Unregister(() => { UnRegister(call); });
            if (PiEvent.Contains(calls, call))
                unbinder.isEmpty = true;
            else
                calls += call;
            return unbinder;
        }

        public void UnRegister(Action<T> call) => calls -= call;

        public void Invoke(T t)
        {
            actions?.Invoke();
            calls?.Invoke(t);
        }

        public override void UnRegisterAll()
        {
            actions = null;
            calls = null;
        }
    }

    public class PiEvent<T, K> : PiEventBase
    {
        private Action<T, K> calls;

        public IUnRegister Register(Action<T, K> call)
        {
            calls += call;
            return new Unregister(() => { UnRegister(call); });
        }

        /// <summary>
        /// Add event listener nhưng sẽ bỏ qua nếu listener đã được Add.<br/>
        /// Nếu bỏ qua thì IUnRegister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>Instruction to remove listenter</returns>
        public IUnRegister RegisterIfNotExists(Action<T, K> call)
        {
            var unbinder = new Unregister(() => { UnRegister(call); });
            if (PiEvent.Contains(calls, call))
                unbinder.isEmpty = true;
            else
                calls += call;
            return unbinder;
        }

        public void UnRegister(Action<T, K> call) => calls -= call;

        public void Invoke(T t, K k)
        {
            actions?.Invoke();
            calls?.Invoke(t, k);
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

        public IUnRegister Register(Action<T, K, S> call)
        {
            calls += call;
            return new Unregister(() => { UnRegister(call); });
        }

        /// <summary>
        /// Add event listener nhưng sẽ bỏ qua nếu listener đã được Add.<br/>
        /// Nếu bỏ qua thì IUnRegister return sẽ có thuộc tính isEmpty = true.
        /// </summary>
        /// <returns>Instruction to remove listenter</returns>
        public IUnRegister RegisterIfNotExists(Action<T, K, S> call)
        {
            var unbinder = new Unregister(() => { UnRegister(call); });
            if (PiEvent.Contains(calls, call))
                unbinder.isEmpty = true;
            else
                calls += call;
            return unbinder;
        }

        public void UnRegister(Action<T, K, S> call) => calls -= call;

        public void Invoke(T t, K k, S s)
        {
            actions?.Invoke();
            calls?.Invoke(t, k, s);
        }

        public override void UnRegisterAll()
        {
            actions = null;
            calls = null;
        }
    }
}