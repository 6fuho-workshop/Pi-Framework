//using UnityEngine;
//using System.Collections;
namespace PiFramework
{
    public class PiEvent
    {
        private string _name;
        private object _args;
        private object _sender;
        //private bool _isStoped = false;
        //private bool _isLocked = false;
        //public object _currentTarget;


        //--------------------------------------
        // INITIALIZE
        //--------------------------------------

        public PiEvent(string name, object sender, object args)
        {
            _name = name;
            _sender = sender;
            _args = args;
        }

        public PiEvent(string name)
        {
            _name = name;
        }

        public string Name { get { return _name; } }
        public object Args { get { return _args; } }
        public object Sender { get { return _sender; } }

    }
}