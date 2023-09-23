using UnityEngine;
using System.Collections.Generic;
using System;

namespace PiFramework
{
    //public delegate void CommonEventHandler(object sender, object args);
    public delegate void PiEventHandler(PiEvent e);

    public static class PiTMP //this will make inner class hiden from outside
    {
        class EventDriven
        {

            /// <summary>
            /// TODO : chuyển về key int đc thì đỡ tốn ram
            /// TODO : nghiên cứu kỹ hơn về việc chuyển list handler về 1 dạng collection quản lý việc add và invoke handlers,
            /// tránh việc phải clone thành array lúc invoke, chưa biết cách nào tốt hơn
            /// TODO: co van de gi khong khi chuyen List<PiEventHandler> ve dang PiEventHandler only
            /// </summary>
            Dictionary<string, List<PiEventHandler>> _handlers = new Dictionary<string, List<PiEventHandler>>();

            //
            public void RaiseEvent(PiEvent e)
            {
                List<PiEventHandler> tmp;
                if (_handlers.TryGetValue(e.Name, out tmp))
                {
                    PiEventHandler[] handlers = tmp.ToArray(); // avoid modifying list while performing iterate
                    foreach (var handler in handlers)
                        handler.Invoke(e);
                }
            }

            /*
            public void RaiseEvent(string name, object sender)
            {
                RaiseEvent(name, sender, null);
            }

            public void RaiseEvent(string name)
            {
                RaiseEvent(name, null, null);
            }
            */
            public void AddEventHandler(string name, PiEventHandler handler)
            {
                List<PiEventHandler> tmp;
                if (_handlers.TryGetValue(name, out tmp))
                {
                    tmp.Add(handler);
                }
                else
                {
                    _handlers[name] = new List<PiEventHandler>();
                    _handlers[name].Add(handler);
                }
            }

            public void RemoveEventHandler(string name, PiEventHandler handler)
            {
                List<PiEventHandler> tmp;
                if (_handlers.TryGetValue(name, out tmp))
                {
                    tmp.Remove(handler);
                }
            }


        }
    }
}