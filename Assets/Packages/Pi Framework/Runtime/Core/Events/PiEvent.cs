
using PiFramework.Mediator;
using System;

namespace PiFramework
{
    public interface IPiEvent
    {
        IUnRegister AddListener(Action call);
        void RemoveAllListeners();
    }

    public class PiEvent : IPiEvent
    {
        private Action mCalls;

        public IUnRegister AddListener(Action call)
        {
            mCalls += call;
            return new UnRegister(() => { RemoveListener(call); });
        }

        public void RemoveListener(Action onEvent) => mCalls -= onEvent;

        public void Invoke() => mCalls?.Invoke();

        public void RemoveAllListeners() => mCalls = null;
    }

    public class PiEvent<T> : IPiEvent
    {
        private Action<T> mCalls;

        public IUnRegister AddListener(Action<T> call)
        {
            mCalls += call;
            return new UnRegister(() => { RemoveListener(call); });
        }

        public void RemoveListener(Action<T> call) => mCalls -= call;

        public void Invoke(T t) => mCalls?.Invoke(t);

        IUnRegister IPiEvent.AddListener(Action call)
        {
            return AddListener(Action);
            void Action(T _) => call();
        }

        public void RemoveAllListeners() => mCalls = null;
    }

    public class PiEvent<T, K> : IPiEvent
    {
        private Action<T, K> mCalls;

        public IUnRegister AddListener(Action<T, K> call)
        {
            mCalls += call;
            return new UnRegister(() => { RemoveListener(call); });
        }

        public void RemoveListener(Action<T, K> call) => mCalls -= call;

        public void Invoke(T t, K k) => mCalls?.Invoke(t, k);

        IUnRegister IPiEvent.AddListener(Action call)
        {
            return AddListener(Action);
            void Action(T _, K __) => call();
        }

        public void RemoveAllListeners() => mCalls = null;
    }

    public class PiEvent<T, K, S> : IPiEvent
    {
        private Action<T, K, S> mCalls;

        public IUnRegister AddListener(Action<T, K, S> call)
        {
            mCalls += call;
            return new UnRegister(() => { RemoveListener(call); });
        }

        public void RemoveListener(Action<T, K, S> call) => mCalls -= call;

        public void Invoke(T t, K k, S s) => mCalls?.Invoke(t, k, s);

        IUnRegister IPiEvent.AddListener(Action call)
        {
            return AddListener(Action);
            void Action(T _, K __, S ___) => call();
        }

        public void RemoveAllListeners() => mCalls = null;
    }
}