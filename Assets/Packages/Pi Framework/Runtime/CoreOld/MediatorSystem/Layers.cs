using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PF.Mediator
{
    #region Controller

    public interface IController : ICanGetMediator, ICanSendCommand, ICanGetSystem, ICanGetModel,
        ICanSubscribeEvent, ICanSendQuery, ICanGetUtility, ICanHandleCommand
    {
    }

    #endregion

    #region System

    public interface ISystem : ICanGetMediator, ICanSetMediator, ICanGetModel, ICanGetUtility,
        ICanSubscribeEvent, ICanSendEvent, ICanGetSystem, ICanHandleCommand
    {
        void Init();
    }

    public abstract class AbstractSystem : ISystem
    {
        private IMediator mMediator;

        IMediator ICanGetMediator.GetMediator() => mMediator;

        void ICanSetMediator.SetMediator(IMediator mediator) => mMediator = mediator;

        void ISystem.Init() => OnInit();

        protected abstract void OnInit();
    }

    public abstract class MonoSystem : MonoBehaviour, ISystem
    {
        private IMediator mMediator;

        IMediator ICanGetMediator.GetMediator() => mMediator;

        void ICanSetMediator.SetMediator(IMediator mediator) => mMediator = mediator;

        void ISystem.Init() => OnInit();

        protected abstract void OnInit();
    }

    #endregion

    #region Model

    public interface IModel : ICanGetMediator, ICanSetMediator, ICanGetUtility, ICanSendEvent, ICanHandleCommand
    {
        void Init();
    }

    public abstract class AbstractModel : IModel
    {
        private IMediator mMediator;

        IMediator ICanGetMediator.GetMediator() => mMediator;

        void ICanSetMediator.SetMediator(IMediator mediator) => mMediator = mediator;

        void IModel.Init() => OnInit();

        protected abstract void OnInit();
    }

    #endregion

    #region Utility

    public interface IUtility
    {
    }

    #endregion
}