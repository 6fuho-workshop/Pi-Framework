using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiEditor.Callbacks
{
    public class OnLoadPiEditorAttribute : CallbackOrderAttribute
    {
        public OnLoadPiEditorAttribute()
        {
            _callbackOrder = 1;
        }

        public OnLoadPiEditorAttribute(int callbackOrder)
        {
            _callbackOrder = callbackOrder;
        }
    }
}