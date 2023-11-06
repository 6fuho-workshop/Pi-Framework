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

        public IUnRegister AddListener(Action call)
        {
            mCalls += call;
            return new UnRegister(() => { RemoveListener(call); });
        }

        public void RemoveListener(Action call)
        {
            mCalls -= call;
            ChainRemoveListener();
        }

        private void Invoke() => mCalls?.Invoke();

        private List<IUnRegister> _unRegisterList { get; } = new List<IUnRegister>();

        void ChainRemoveListener()
        {
            _unRegisterList.ForEach(x => x.ExecUnRegister());
            _unRegisterList.Clear();
        }
    }

    public static class PiEventChainExtensions
    {
        public static PiEventChain Chain(this IPiEvent self, IPiEvent e) => new PiEventChain().Chain(self).Chain(e);
    }
}