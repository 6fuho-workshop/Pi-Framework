using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PF.PiEditor.Callbacks
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

    public class OnPiImportAttribute : CallbackOrderAttribute
    {
        public OnPiImportAttribute()
        {
            _callbackOrder = 1;
        }

        public OnPiImportAttribute(int callbackOrder)
        {
            _callbackOrder = callbackOrder;
        }
    }

    public class OnPiSetupAttribute : CallbackOrderAttribute
    {
        public OnPiSetupAttribute()
        {
            _callbackOrder = 1;
        }

        public OnPiSetupAttribute(int callbackOrder)
        {
            _callbackOrder = callbackOrder;
        }
    }
}