using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


namespace PiFramework.Mediator
{

    #region Mediator

    public interface IMediator
    {
        void RegisterSystem<T>(T system) where T : ISystem;

        void RegisterModel<T>(T model) where T : IModel;

        void RegisterUtility<T>(T utility) where T : IUtility;

        T GetSystem<T>() where T : class, ISystem;

        T GetModel<T>() where T : class, IModel;

        T GetUtility<T>() where T : class, IUtility;

        void SendCommand<T>(T command) where T : ICommand;

        TResult SendCommand<TResult>(ICommand<TResult> command);

        TResult SendQuery<TResult>(IQuery<TResult> query);

        void SendEvent<T>() where T : new();
        void SendEvent<T>(T e);

        IUnregister AddListener<T>(Action<T> onEvent);
        void RemoveListener<T>(Action<T> onEvent);

        void Destroy();
    }

    public abstract class Mediator<T> : IMediator where T : Mediator<T>, new()
    {
        private bool initialized = false;

        private HashSet<ISystem> systems = new();

        private HashSet<IModel> models = new();

        public static PiEvent<T> patchingRegisters;

        public static IUnregisterList unregisterList = new UnregisterList();

        protected static T _instance;

        public static IMediator instance
        {
            get
            {
                if (_instance == null)
                {
                    InstantiateMediator();
                }

                return _instance;
            }
        }


        static void InstantiateMediator()
        {

            _instance = new T();
            _instance.Init();

            patchingRegisters?.Invoke(_instance);
            patchingRegisters = null;

            foreach (var model in _instance.models)
            {
                model.Init();
            }

            _instance.models.Clear();

            foreach (var system in _instance.systems)
            {
                system.Init();
            }

            _instance.systems.Clear();
            _instance.initialized = true;
        }

        public void Destroy()
        {
            container.Clear();
            typeEventSystem.Clear();
            patchingRegisters = null;
            unregisterList.UnregisterAll();
            _instance = null;
        }

        protected abstract void Init();

        private IOCContainer container = new();

        public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            system.SetMediator(this);
            container.Register<TSystem>(system);

            if (!initialized)
            {
                systems.Add(system);
            }
            else
            {
                system.Init();
            }
        }

        public void RegisterModel<TModel>(TModel model) where TModel : IModel
        {
            model.SetMediator(this);
            container.Register<TModel>(model);

            if (!initialized)
            {
                models.Add(model);
            }
            else
            {
                model.Init();
            }
        }

        public void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility =>
            container.Register<TUtility>(utility);

        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem => container.Get<TSystem>();

        public TModel GetModel<TModel>() where TModel : class, IModel => container.Get<TModel>();

        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility => container.Get<TUtility>();

        public TResult SendCommand<TResult>(ICommand<TResult> command) => ExecuteCommand(command);

        public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand => ExecuteCommand(command);

        protected virtual TResult ExecuteCommand<TResult>(ICommand<TResult> command)
        {
            command.SetMediator(this);
            return command.Execute();
        }

        protected virtual void ExecuteCommand(ICommand command)
        {
            command.SetMediator(this);
            command.Execute();
        }

        public TResult SendQuery<TResult>(IQuery<TResult> query) => DoQuery<TResult>(query);

        protected virtual TResult DoQuery<TResult>(IQuery<TResult> query)
        {
            query.SetMediator(this);
            return query.Do();
        }

        private TypeEventSystem typeEventSystem = new();

        public void SendEvent<TEvent>() where TEvent : new() => typeEventSystem.Send<TEvent>();

        public void SendEvent<TEvent>(TEvent e) => typeEventSystem.Send<TEvent>(e);

        public IUnregister AddListener<TEvent>(Action<TEvent> onEvent) => typeEventSystem.AddListener<TEvent>(onEvent);

        public void RemoveListener<TEvent>(Action<TEvent> onEvent) => typeEventSystem.RemoveListener<TEvent>(onEvent);
    }

    #endregion

    #region IOC

    public class IOCContainer
    {
        private Dictionary<Type, object> instances = new();

        public void Register<T>(T instance)
        {
            var key = typeof(T);

            if (instances.ContainsKey(key))
            {
                instances[key] = instance;
            }
            else
            {
                instances.Add(key, instance);
            }
        }

        public T Get<T>() where T : class
        {
            var key = typeof(T);

            if (instances.TryGetValue(key, out var retInstance))
            {
                return retInstance as T;
            }

            return null;
        }

        public void Clear()
        {
            instances.Clear();
        }
    }

    #endregion

    #region Rule

    public interface ICanGetMediator
    {
        IMediator GetMediator();
    }

    public interface ICanSetMediator
    {
        void SetMediator(IMediator mediator);
    }

    public interface ICanGetModel : ICanGetMediator
    {
    }

    public static class CanGetModelExtension
    {
        public static T GetModel<T>(this ICanGetModel self) where T : class, IModel =>
            self.GetMediator().GetModel<T>();
    }

    public interface ICanGetSystem : ICanGetMediator
    {
    }

    public static class CanGetSystemExtension
    {
        public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem =>
            self.GetMediator().GetSystem<T>();
    }

    public interface ICanGetUtility : ICanGetMediator
    {
    }

    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility =>
            self.GetMediator().GetUtility<T>();
    }

    public interface ICanAddEventListener : ICanGetMediator
    {
    }

    public static class CanAddEventListenerExtension
    {
        public static IUnregister AddListener<T>(this ICanAddEventListener self, Action<T> onEvent) =>
            self.GetMediator().AddListener<T>(onEvent);

        public static void RemoveListener<T>(this ICanAddEventListener self, Action<T> onEvent) =>
            self.GetMediator().RemoveListener<T>(onEvent);
    }

    public interface ICanSendCommand : ICanGetMediator
    {
    }

    public static class CanSendCommandExtension
    {
        public static void SendCommand<T>(this ICanSendCommand self) where T : ICommand, new() =>
            self.GetMediator().SendCommand<T>(new T());

        public static void SendCommand<T>(this ICanSendCommand self, T command) where T : ICommand =>
            self.GetMediator().SendCommand<T>(command);

        public static TResult SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command) =>
            self.GetMediator().SendCommand(command);
    }

    public interface ICanSendEvent : ICanGetMediator
    {
    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T>(this ICanSendEvent self) where T : new() =>
            self.GetMediator().SendEvent<T>();

        public static void SendEvent<T>(this ICanSendEvent self, T e) => self.GetMediator().SendEvent<T>(e);
    }

    public interface ICanSendQuery : ICanGetMediator
    {
    }

    public static class CanSendQueryExtension
    {
        public static TResult SendQuery<TResult>(this ICanSendQuery self, IQuery<TResult> query) =>
            self.GetMediator().SendQuery(query);
    }

    #endregion

}