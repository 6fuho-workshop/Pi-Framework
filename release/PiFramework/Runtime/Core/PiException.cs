using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PiFramework
{
    public class PiException : Exception
    {
        public PiException(string s) : base(s)
        { 
        }

    }
}
