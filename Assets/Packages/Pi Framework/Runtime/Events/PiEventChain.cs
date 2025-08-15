using PF.Mediator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PF.Core.Common;

namespace PF
{
    /// <summary>
    /// Represents a chainable event system that allows registering, invoking, and managing event listeners.
    /// Đóng gói các sự kiện thành một nhóm, cho phép add và remove listener của tất cả các sự kiện trong nhóm đó.
    /// </summary>
    /// <remarks>The <see cref="PiEventChain"/> class provides functionality to chain events, add listeners,
    /// and manage their lifecycle. It supports chaining multiple events together and ensures proper cleanup of
    /// listeners when they are removed.</remarks>
    public class PiEventChain
    {
        public PiEventChain Chain(IPiEvent piEvent)
        {
            unbinderList.Add(piEvent.Register(Invoke));
            return this;
        }

        private Action mCalls;

        public IUnregister AddListener(Action call)
        {
            mCalls += call;
            return new CustomUnregister(() => { RemoveListener(call); });
        }

        public void RemoveListener(Action call)
        {
            mCalls -= call;
            ChainRemoveListener();
        }

        private void Invoke() => mCalls?.Invoke();

        private List<IUnregister> unbinderList { get; } = new List<IUnregister>();

        void ChainRemoveListener()
        {
            unbinderList.ForEach(x => x.Unregister());
            unbinderList.Clear();
        }
    }

    public static class PiEventChainExtensions
    {
        public static PiEventChain Chain(this IPiEvent self, IPiEvent e) => new PiEventChain().Chain(self).Chain(e);
    }
}