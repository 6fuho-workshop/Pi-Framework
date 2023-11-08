using PiFramework.Mediator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public class PiEventChain
    {
        public PiEventChain Chain(IPiEvent piEvent)
        {
            _unRegisterList.Add(piEvent.AddListener(Invoke));
            return this;
        }

        private Action mCalls = () => { };

        public IUnregister AddListener(Action call)
        {
            mCalls += call;
            return new Unregister(() => { RemoveListener(call); });
        }

        public void RemoveListener(Action call)
        {
            mCalls -= call;
            ChainRemoveListener();
        }

        private void Invoke() => mCalls?.Invoke();

        private List<IUnregister> _unRegisterList { get; } = new List<IUnregister>();

        void ChainRemoveListener()
        {
            _unRegisterList.ForEach(x => x.InvokeUnregister());
            _unRegisterList.Clear();
        }
    }

    public static class PiEventChainExtensions
    {
        public static PiEventChain Chain(this IPiEvent self, IPiEvent e) => new PiEventChain().Chain(self).Chain(e);
    }
}