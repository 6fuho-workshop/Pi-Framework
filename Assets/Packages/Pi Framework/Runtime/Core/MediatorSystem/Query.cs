using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.Mediator
{
    public interface IQuery<TResult> : ICanGetMediator, ICanSetMediator, ICanGetModel, ICanGetSystem,
        ICanSendQuery
    {
        TResult Do();
    }

    public abstract class AbstractQuery<T> : IQuery<T>
    {
        public T Do() => OnDo();

        protected abstract T OnDo();


        private IMediator mMediator;

        public IMediator GetMediator() => mMediator;

        public void SetMediator(IMediator mediator) => mMediator = mediator;
    }
}
