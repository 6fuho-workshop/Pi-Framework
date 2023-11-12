using UnityEngine;
using System.Collections;
using System;

namespace PiFramework
{
    public class PiConsoleResult
    {
        //public static readonly PiConsoleResult Dummy = new PiConsoleResult();
        public string Output;
        public bool Error;
        public string Message;
        public int Code;
        public bool IsDone;

        public void Reset(){
            Output = string.Empty;
            Message = string.Empty;
            Code = 0;
            Error = false;
        }
    }
}