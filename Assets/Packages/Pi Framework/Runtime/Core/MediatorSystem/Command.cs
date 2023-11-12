using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.Mediator
{
    public interface ICommand : ICanGetMediator, ICanSetMediator, ICanGetSystem, ICanGetModel, ICanGetUtility,
        ICanSendEvent, ICanSendCommand, ICanSendQuery
    {
        void Execute();
    }

    public interface ICommand<TResult> : ICanGetMediator, ICanSetMediator, ICanGetSystem, ICanGetModel,
        ICanGetUtility, ICanSendEvent, ICanSendCommand, ICanSendQuery
    {
        TResult Execute();
    }

    public abstract class AbstractCommand : ICommand
    {
        private IMediator mMediator;

        IMediator ICanGetMediator.GetMediator() => mMediator;

        void ICanSetMediator.SetMediator(IMediator mediator) => mMediator = mediator;

        void ICommand.Execute() => OnExecute();

        protected abstract void OnExecute();
    }

    public abstract class AbstractCommand<TResult> : ICommand<TResult>
    {
        private IMediator mMediator;

        IMediator ICanGetMediator.GetMediator() => mMediator;

        void ICanSetMediator.SetMediator(IMediator mediator) => mMediator = mediator;

        TResult ICommand<TResult>.Execute() => OnExecute();

        protected abstract TResult OnExecute();
    }
}
