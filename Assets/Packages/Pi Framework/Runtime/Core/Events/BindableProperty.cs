using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PiFramework
{
    public interface IBindableProperty<T> : IReadonlyBindableProperty<T>
    {
        new T value { get; set; }
        void SetValueIgnoreEvent(T newValue);
    }

    public interface IReadonlyBindableProperty<T> : IPiEvent
    {
        T value { get; }

        /// <summary>
        /// Pass current value to callback and Invoke callback to notify as changed event
        /// </summary>
        IUnRegister RegisterNotifyBack(Action<T> changedHandler);
        void UnRegister(Action<T> changedHandler);
        IUnRegister Register(Action<T> changedHandler);
    }

    /// <summary>
    /// BindableProperty is an object containing data + data change events.
    /// </summary>
    /// <typeparam name="T">Property Data Type</typeparam>
    public class BindableProperty<T> : IBindableProperty<T>
    {
        public BindableProperty(T defaultValue = default) => _value = defaultValue;

        protected T _value;

        public static Func<T, T, bool> Comparer { get; set; } = (a, b) => a.Equals(b);

        public BindableProperty<T> WithComparer(Func<T, T, bool> comparer)
        {
            Comparer = comparer;
            return this;
        }

        public T value
        {
            get => GetValue();
            set
            {
                if (value == null && this._value == null) return;
                if (value != null && Comparer(value, _value)) return;

                SetValue(value);
                valueChanged?.Invoke(value);
                valueChangedAction.Invoke();
            }
        }

        protected virtual void SetValue(T newValue) => _value = newValue;

        protected virtual T GetValue() => _value;

        public void SetValueIgnoreEvent(T newValue) => _value = newValue;

        private Action<T> valueChanged;
        private Action valueChangedAction;

        public IUnRegister Register(Action<T> changedHandler)
        {
            valueChanged += changedHandler;
            //return new BindablePropertyUnRegister<T>(this, changedHandler);
            return new Unregister(() => UnRegister(changedHandler));
        }

        public IUnRegister RegisterIfNotExists(Action<T> changedHandler)
        {
            //var unbinder = new BindablePropertyUnRegister<T>(this, onValueChanged);

            var unbinder = new Unregister(() => UnRegister(changedHandler));

            if (PiEvent.Contains(valueChanged, changedHandler))
                unbinder.isEmpty = true;
            else
                valueChanged += changedHandler;

            return unbinder;
        }

        public IUnRegister RegisterNotifyBack(Action<T> onValueChanged)
        {
            var unregister = Register(onValueChanged);
            onValueChanged(_value);
            return unregister;
        }

        public void UnRegister(Action<T> onValueChanged) => valueChanged -= onValueChanged;

        public IUnRegister Register(Action onChangedAction)
        {
            valueChangedAction += onChangedAction;
            return new Unregister(() => UnRegister(onChangedAction));
            //return AddListener(Action);
            //void Action(T _) => call();
        }

        public IUnRegister RegisterIfNotExists(Action onChangedAction)
        {
            var unbinder = new Unregister(() => UnRegister(onChangedAction));

            if (PiEvent.Contains(valueChangedAction, onChangedAction))
                unbinder.isEmpty = true;
            else
                valueChangedAction += onChangedAction;

            return unbinder;
        }

        public void UnRegister(Action onChangedAction) => valueChangedAction -= onChangedAction;

        public override string ToString() => value.ToString();

        internal void UnRegisterAll()
        {
            valueChanged = null;
            valueChangedAction = null;
        }
    }

    internal class ComparerAutoRegister
    {
#if UNITY_5_6_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoRegister()
        {
            BindableProperty<int>.Comparer = (a, b) => a == b;
            BindableProperty<float>.Comparer = (a, b) => a == b;
            BindableProperty<double>.Comparer = (a, b) => a == b;
            BindableProperty<string>.Comparer = (a, b) => a == b;
            BindableProperty<long>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Vector2>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Vector3>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Vector4>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Color>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Color32>.Comparer = (a, b) => a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
            BindableProperty<UnityEngine.Bounds>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Rect>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Quaternion>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Vector2Int>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.Vector3Int>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.BoundsInt>.Comparer = (a, b) => a == b;
            BindableProperty<UnityEngine.RangeInt>.Comparer = (a, b) => a.start == b.start && a.length == b.length;
            BindableProperty<UnityEngine.RectInt>.Comparer = (a, b) => a.Equals(b);
        }
#endif
    }

    /*
    public class BindablePropertyUnRegister<T> : IUnRegister
    {
        public BindablePropertyUnRegister(BindableProperty<T> bindableProperty, Action<T> valueChanged)
        {
            this.bindableProperty = bindableProperty;
            this.valueChanged = valueChanged;
        }

        public BindableProperty<T> bindableProperty { get; set; }

        public Action<T> valueChanged { get; set; }
        public bool isEmpty { get; set; }

        public void UnRegister()
        {
            bindableProperty.UnRegister(valueChanged);
            bindableProperty = null;
            valueChanged = null;
        }
    }
    */
}