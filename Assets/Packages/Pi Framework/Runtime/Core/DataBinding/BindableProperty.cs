using PiFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PiFramework
{
    public interface IBindableProperty<T> : IReadonlyBindableProperty<T>
    {
        new T Value { get; set; }
        void SetValueWithoutEvent(T newValue);
    }

    public interface IReadonlyBindableProperty<T> : IPiEvent
    {
        T Value { get; }

        IUnRegister AddListenerWithCallBack(Action<T> action);
        void RemoveListener(Action<T> onValueChanged);
        IUnRegister AddListener(Action<T> onValueChanged);
    }

    public class BindableProperty<T> : IBindableProperty<T>
    {
        public BindableProperty(T defaultValue = default) => value = defaultValue;

        protected T value;

        public static Func<T, T, bool> Comparer { get; set; } = (a, b) => a.Equals(b);

        public BindableProperty<T> WithComparer(Func<T, T, bool> comparer)
        {
            Comparer = comparer;
            return this;
        }

        public T Value
        {
            get => GetValue();
            set
            {
                if (value == null && this.value == null) return;
                if (value != null && Comparer(value, this.value)) return;

                SetValue(value);
                onValueChanged?.Invoke(value);
            }
        }

        protected virtual void SetValue(T newValue) => value = newValue;

        protected virtual T GetValue() => value;

        public void SetValueWithoutEvent(T newValue) => value = newValue;

        private Action<T> onValueChanged = (v) => { };

        public IUnRegister AddListener(Action<T> onValueChanged)
        {
            this.onValueChanged += onValueChanged;
            return new BindablePropertyUnRegister<T>(this, onValueChanged);
        }

        public IUnRegister AddListenerWithCallBack(Action<T> onValueChanged)
        {
            onValueChanged(value);
            return AddListener(onValueChanged);
        }

        public void RemoveListener(Action<T> onValueChanged) => this.onValueChanged -= onValueChanged;

        IUnRegister IPiEvent.AddListener(Action onEvent)
        {
            return AddListener(Action);
            void Action(T _) => onEvent();
        }

        public override string ToString() => Value.ToString();

        public void RemoveAllListeners() => onValueChanged = (v) => { };
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

    public class BindablePropertyUnRegister<T> : IUnRegister
    {
        public BindablePropertyUnRegister(BindableProperty<T> bindableProperty, Action<T> onValueChanged)
        {
            BindableProperty = bindableProperty;
            OnValueChanged = onValueChanged;
        }

        public BindableProperty<T> BindableProperty { get; set; }

        public Action<T> OnValueChanged { get; set; }

        public void ExecUnRegister()
        {
            BindableProperty.RemoveListener(OnValueChanged);
            BindableProperty = null;
            OnValueChanged = null;
        }
    }
}